using System.Runtime.InteropServices;
using AutoMapper;
using Formationn_Ecommerce.Application.Cart.Dtos;
using Formationn_Ecommerce.Application.Cart.Interfaces;
using Formationn_Ecommerce.Application.Coupons.Interfaces;
using Formationn_Ecommerce.Application.Order.Dtos;
using Formationn_Ecommerce.Application.Order.Interfaces;
using Formationn_Ecommerce.Core.Entities.Identity;
using Formationn_Ecommerce.Core.Interfaces.Repositories;
using Formationn_Ecommerce.Core.Not_Mapped_Entities;
using Formationn_Ecommerce.Core.Utility;
using Formationn_Ecommerce.Infrastucture.Persistence.Repository;
using Formationn_Ecommerce.Models.Cart;
using Formationn_Ecommercee.Core.Interfaces.External;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace Formationn_Ecommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;
        private readonly IOrderServices _orderServices;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly IStripePayment _stripePayment;
        
        public CartController(ICartService cartService,
                              ICouponService couponService,
                              IOrderServices orderServices,
                              SignInManager<ApplicationUser> signInManager,
                              IMapper mapper,
                              IStripePayment stripePayment)
        {
            _cartService = cartService;
            _couponService = couponService;
            _orderServices = orderServices;
            _signInManager = signInManager;
            _mapper = mapper;
            _stripePayment = stripePayment;
        }
        public async Task<IActionResult> CartIndex()
        {
            var cartDto = await LoadCartDtoBasedOnLoggedInUser();
            if (cartDto == null)
            {
                return View(new CartIndexViewModel());
            }
            CartIndexViewModel cartIndexViewModel = _mapper.Map<CartIndexViewModel>(cartDto);
            return View(cartIndexViewModel);
        }

        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            // Retrive the user from SinInManager
            var user = await _signInManager.UserManager.GetUserAsync(User);

            if (string.IsNullOrEmpty(user.Id))
            {
                return new CartDto();
            }

            CartDto? cart = await _cartService.GetCartByUserIdAsync(user.Id);

            if (cart == null)
            {
                return new CartDto();
            }

            return cart;
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartIndexViewModel cartIndexViewModel)
        {
            var couponCode = cartIndexViewModel.CartHeader.CouponCode;
            if (string.IsNullOrEmpty(couponCode))
            {
                TempData["error"] = "Coupon invalide";
                return RedirectToAction("CartIndex", "Cart");
            }
            
            try
            {
                //Check valid coupon
                var existingCoupon = await _couponService.GetCouponByCodeAsync(couponCode);
                if (existingCoupon == null)
                {
                    TempData["error"] = "Coupon invalide";
                    return RedirectToAction(nameof(CartIndex));
                }
                
                //Retrive the user from SinInManager
                var user = await _signInManager.UserManager.GetUserAsync(User);
                var cartDto = await _cartService.ApplyCouponAsync(user.Id, couponCode);

                if (cartDto != null)
                {
                    TempData["success"] = "Coupon appliqué avec succès";
                    return RedirectToAction(nameof(CartIndex));
                }

                TempData["error"] = "Une erreur est survenue lors de l'application du coupon";
                return RedirectToAction(nameof(CartIndex));
            }
            catch (InvalidOperationException ex)
            {
                // Capture les erreurs spécifiques à Stripe ou à la validation des coupons
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(CartIndex));
            }
            catch (Exception ex)
            {
                TempData["error"] = "Une erreur est survenue lors de l'application du coupon";
                return RedirectToAction(nameof(CartIndex));
            }
        }

        public async Task<IActionResult> RemoveCoupon()
        {

            // Retrive the user from SinInManager
            var user = await _signInManager.UserManager.GetUserAsync(User);
            var cartDto = await _cartService.ApplyCouponAsync(user.Id, "");

            if (cartDto != null)
            {
                TempData["success"] = "Coupon Removed Successfully";
                return RedirectToAction(nameof(CartIndex));
            }

            TempData["error"] = "An error occurred while updating the cart.";
            return RedirectToAction(nameof(CartIndex));
        }

        public async Task<IActionResult> Checkout()
        {
            var cartDto = await LoadCartDtoBasedOnLoggedInUser();
            if (cartDto == null)
            {
                return View(new CartIndexViewModel());
            }
            CartIndexViewModel cartIndexViewModel = _mapper.Map<CartIndexViewModel>(cartDto);
            return View(cartIndexViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CartIndexViewModel cartIndexViewModel)
        {
            // Vérifier si les données du modèle sont présentes
            if (cartIndexViewModel.CartDetails == null || !cartIndexViewModel.CartDetails.Any())
            {
                TempData["error"] = "Les détails du panier n'ont pas été transmis. Veuillez réessayer.";
                return RedirectToAction(nameof(Checkout));
            }

            // Récupérer l'utilisateur
            var user = await _signInManager.UserManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Veuillez vous connecter pour finaliser votre commande";
                return RedirectToAction("Login", "Account");
            }

            // Assigner l'ID utilisateur à l'en-tête de commande
            var orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartIndexViewModel.CartHeader);
            orderHeaderDto.UserId = user.Id;
            orderHeaderDto.Status = StaticDetails.Status_Pending;
            orderHeaderDto.OrderTime = DateTime.Now;
            orderHeaderDto.OrderTotal = cartIndexViewModel.CartHeader.CartTotal;

            // Créer une liste pour pouvoir modifier les détails après la création de l'en-tête
            var orderDetailsDto = _mapper.Map<List<OrderDetailsDto>>(cartIndexViewModel.CartDetails);
            orderHeaderDto.OrderDetails = orderDetailsDto;
            try
            {
                var orderHeaderFromDb = await _orderServices.AddOrderHeaderAsync(orderHeaderDto);
                if (orderHeaderFromDb != null)
                {
                    // Mettre à jour l'OrderHeaderId pour chaque détail de commande
                    foreach (var detail in orderDetailsDto)
                    {
                        detail.OrderHeaderId = orderHeaderFromDb.Id;
                    }

                    var orderDetailsFromDb = await _orderServices.AddOrderDetailsAsync(orderDetailsDto);
                    if (orderDetailsFromDb != null)
                    {
                        // Récupérer la session Stripe créée lors de l'ajout de la commande
                        var orderHeader = await _orderServices.GetOrderByIdAsync(orderHeaderFromDb.Id);
                        
                        if (!string.IsNullOrEmpty(orderHeader?.StripeSessionId))
                        {
                            // Rediriger vers la page de paiement Stripe
                            var session = await new SessionService().GetAsync(orderHeader.StripeSessionId);

                            // Vider le panier avant de rediriger
                            await _cartService.ClearCartAsync(user.Id);

                            return Redirect(session.Url); // Redirection HTTP 302 vers l'URL Stripe
                        }
                        else
                        {
                            TempData["success"] = "Commande créée avec succès";
                            return RedirectToAction("OrderConfirmation", "Order", new { id = orderHeaderFromDb.Id });
                        }
                    }
                    else
                    {
                        TempData["error"] = "Erreur lors de la création des détails de la commande";
                        return View(cartIndexViewModel);
                    }
                }
                else
                {
                    TempData["error"] = "Erreur lors de la création de l'en-tête de la commande";
                    return View(cartIndexViewModel);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Une erreur s'est produite: {ex.Message}";
                return View(cartIndexViewModel);
            }
        }
    }
}
