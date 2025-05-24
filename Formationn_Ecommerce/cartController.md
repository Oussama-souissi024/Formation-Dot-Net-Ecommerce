# CartController Implementation Guide

This document explains how to fully implement `CartController.cs` for the **Formationn _Ecommerce** solution, based on the domain / application model already present (entities, repositories, services) and inspired by the controller of the legacy project you provided.

---

## 1. Responsibilities
The controller acts as the presentation-layer façade for all shopping-cart-related use-cases:

1. Display the current cart (`Index`).
2. Add / remove coupons (`ApplyCoupon`, `RemoveCoupon`).
3. Remove a single line item (`Remove`).
4. Checkout (**GET** shows summary, **POST** creates order + Stripe session).
5. Handle post-payment confirmation (`Confirmation`).

All heavy business-logic is delegated to **Application-layer** services so that the controller is kept thin.

---

## 2. Dependencies to inject (via constructor)
```csharp
private readonly ICartService _cartService;          // Application layer (already exists)
private readonly SignInManager<ApplicationUser> _signInManager; // For current user
// TODO: create these two application-layer facades
private readonly IOrderService _orderService;        // Order workflow
private readonly IPaymentSession _paymentSession;    // Stripe or other PSP
private readonly IConfiguration _configuration;      // Misc settings (Stripe keys, etc.)
```

> **Note**
> • `ICartService` exists in *Formationn_Ecommerce.Application.Cart.Interfaces*.
> • `IOrderService` & `IPaymentSession` should be added later inside *Application* layer.

---

## 3. Route & authorization attributes
```csharp
[Authorize]               // Optional – depends on functional rules
[Route("[controller]")]  // => /Cart
public class CartController : Controller { /* … */ }
```

---

## 4. Complete controller skeleton
```csharp
using Formationn_Ecommerce.Application.Cart.Dtos;
using Formationn_Ecommerce.Application.Cart.Interfaces;
using Formationn_Ecommerce.Application.Orders.Interfaces;      // you will create
using Formationn_Ecommerce.Application.Payment.Interfaces;     // you will create
using Formationn_Ecommerce.Core.Entities.Identity;             // ApplicationUser
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Formationn_Ecommerce.Controllers;

[Route("[controller]")]
public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IOrderService _orderService;
    private readonly IPaymentSession _paymentSession;
    private readonly IConfiguration _configuration;

    public CartController(
        ICartService cartService,
        SignInManager<ApplicationUser> signInManager,
        IOrderService orderService,
        IPaymentSession paymentSession,
        IConfiguration configuration)
    {
        _cartService     = cartService;
        _signInManager   = signInManager;
        _orderService    = orderService;
        _paymentSession  = paymentSession;
        _configuration   = configuration;
    }

    // ----------------------------
    // 1) CART INDEX (GET)
    // ----------------------------
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var cart = await LoadCartOfCurrentUserAsync();
        return View(cart);
    }

    private async Task<CartDto> LoadCartOfCurrentUserAsync()
    {
        var userId = _signInManager.UserManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return new CartDto();

        return await _cartService.GetCartByUserIdAsync(userId) ?? new CartDto();
    }

    // ----------------------------
    // 2) APPLY COUPON (POST)
    // ----------------------------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApplyCoupon(CartDto dto)
    {
        var userId = _signInManager.UserManager.GetUserId(User);
        var updated = await _cartService.ApplyCouponAsync(userId!, dto.CartHeader.CouponCode);
        TempData[updated is not null ? "success" : "error"] = updated is not null ?
            "Coupon appliqué avec succès." :
            "Impossible d'appliquer ce coupon.";
        return RedirectToAction(nameof(Index));
    }

    // ----------------------------
    // 3) REMOVE COUPON (POST)
    // ----------------------------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveCoupon()
    {
        var userId = _signInManager.UserManager.GetUserId(User);
        await _cartService.ApplyCouponAsync(userId!, string.Empty);
        TempData["success"] = "Coupon supprimé.";
        return RedirectToAction(nameof(Index));
    }

    // ----------------------------
    // 4) REMOVE SINGLE LINE (POST)
    // ----------------------------
    [HttpPost]
    public async Task<IActionResult> Remove(Guid cartDetailsId)
    {
        var removed = await _cartService.RemoveCartItemAsync(cartDetailsId);
        TempData[removed ? "success" : "error"] = removed ?
            "Article supprimé." :
            "Impossible de supprimer l'article.";
        return RedirectToAction(nameof(Index));
    }

    // ----------------------------
    // 5) CHECKOUT (GET)
    // ----------------------------
    [HttpGet("Checkout")]
    public async Task<IActionResult> Checkout()
    {
        return View(await LoadCartOfCurrentUserAsync());
    }

    // ----------------------------
    // 6) CHECKOUT (POST)
    // ----------------------------
    [HttpPost("Checkout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckoutPost()
    {
        var cart = await LoadCartOfCurrentUserAsync();
        if (!cart.CartDetails.Any())
        {
            TempData["error"] = "Votre panier est vide.";
            return RedirectToAction(nameof(Index));
        }

        // 6.1 Créer la commande
        var orderHeader = await _orderService.CreateOrderAsync(cart);
        if (orderHeader is null)
        {
            TempData["error"] = "Erreur lors de la création de la commande.";
            return RedirectToAction(nameof(Checkout));
        }

        // 6.2 Créer la session de paiement (Stripe, …)
        var domain = $"{Request.Scheme}://{Request.Host}/";
        var paymentRequest = new PaymentSessionRequest
        {
            OrderHeader      = orderHeader,
            SuccessUrl       = domain + "cart/Confirmation?orderId=" + orderHeader.Id,
            CancelUrl        = domain + "cart/Checkout"
        };
        var payment = await _paymentSession.CreateAsync(paymentRequest);

        if (payment is null)
        {
            TempData["error"] = "Erreur de paiement.";
            return RedirectToAction(nameof(Checkout));
        }

        // 6.3 Stocker la session Stripe & rediriger
        TempData["StripeSessionId"] = payment.SessionId;
        return Redirect(payment.Url);
    }

    // ----------------------------
    // 7) CONFIRMATION (GET)
    // ----------------------------
    [HttpGet("Confirmation")]
    public async Task<IActionResult> Confirmation(int orderId)
    {
        var status = await _paymentSession.ValidateAsync(orderId);
        switch (status?.ToLower())
        {
            case "approved":
                await ClearCurrentCartAsync();
                TempData["success"] = "Paiement accepté. Merci pour votre commande.";
                break;
            case "pending":
                TempData["warning"] = "Paiement en attente.";
                break;
            default:
                TempData["error"] = status ?? "Échec du paiement.";
                return RedirectToAction(nameof(Checkout));
        }
        return View(orderId);
    }

    private async Task ClearCurrentCartAsync()
    {
        var userId = _signInManager.UserManager.GetUserId(User);
        if (!string.IsNullOrEmpty(userId))
            await _cartService.ClearCartAsync(userId);
    }
}
```

---

## 5. View templates to create
| View            | Model       | Purpose                             |
|-----------------|-------------|-------------------------------------|
| `Views/Cart/Index.cshtml`        | `CartDto`  | Overview page / cart management |
| `Views/Cart/Checkout.cshtml`     | `CartDto`  | Confirm shipping & payment info |
| `Views/Cart/Confirmation.cshtml` | `int` (orderId) | Thank-you / status page      |

---

## 6. DI registration (in *Program.cs*)
```csharp
builder.Services.AddScoped<ICartService, CartService>();
// After creating the two services below
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentSession, StripeSession>();
```

---

## 7. Next steps
1. Implement **OrderService** and **StripeSession** in the *Application* layer.
2. Wire Stripe API keys in *appsettings.json* & secrets.
3. Create / complete the Razor views enumerated above.
4. Add unit tests for `CartService` and integration tests for controller routes.

---

🎉 You now have a clear roadmap & reference code to bring **CartController.cs** to life in the new solution.
