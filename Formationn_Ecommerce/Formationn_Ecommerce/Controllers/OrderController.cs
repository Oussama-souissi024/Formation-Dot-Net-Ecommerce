using Formationn_Ecommerce.Application.Cart.Interfaces;
using Formationn_Ecommerce.Application.Order.Dtos;
using Formationn_Ecommerce.Application.Order.Interfaces;
using Formationn_Ecommerce.Application.Order.Services;
using Formationn_Ecommerce.Core.Entities.Identity;
using Formationn_Ecommerce.Core.Utility;
using Formationn_Ecommercee.Core.Interfaces.External;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Climate;

namespace Formationn_Ecommerce.Controllers
{
    public class OrderController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICartService _cartService;
        private readonly IOrderServices _orderServices;
        private readonly IStripePayment _stripePayment;
        
        public OrderController(ICartService cartService,
                               SignInManager<ApplicationUser> signInManager,
                               IOrderServices orderServices,
                               IStripePayment stripePayment)
        {
            _signInManager = signInManager;
            _cartService = cartService;
            _orderServices = orderServices;
            _stripePayment = stripePayment;
        }

        [Authorize]
        public IActionResult OrderIndex()
        {
            return View();
        }

        public async Task<IActionResult> OrderConfirmation(Guid id)
        {
            try
            {
                // Vérifier si l'ID est valide
                var orderHeader = await _orderServices.GetOrderByIdAsync(id);
                if (orderHeader == null)
                {
                    TempData["error"] = "La commande n'a pas été trouvée. Veuillez vérifier votre numéro de commande.";
                    return RedirectToAction("Index", "Home");
                }
                
                // Vérifier et valider le paiement Stripe
                if (!string.IsNullOrEmpty(orderHeader.StripeSessionId))
                {
                    var validatedOrder = await _orderServices.ValidatePaymentAsync(id);
                    
                    if (validatedOrder != null)
                    {
                        if (validatedOrder.Status == "Approved")
                        {
                            TempData["success"] = "Votre paiement a été traité avec succès et votre commande est confirmée!";
                        }
                        else if (validatedOrder.Status == "PaymentRequired")
                        {
                            TempData["warning"] = "Votre paiement est en attente. La commande sera traitée dès réception du paiement.";
                        }
                        else if (validatedOrder.Status == "PaymentFailed")
                        {
                            TempData["error"] = "Le paiement a échoué. Veuillez réessayer ou contacter le service client.";
                        }
                    }
                }
                else
                {
                    // Pour les commandes sans paiement Stripe
                    TempData["success"] = "Votre commande a été enregistrée avec succès et sera traitée rapidement!";
                }
                
                // Convertir l'ID Guid en int pour la vue
                // La vue s'attend à recevoir un simple entier comme modèle
                return View(id.GetHashCode());
            }
            catch (Exception ex)
            {
                // En cas d'erreur, afficher un message d'erreur et rediriger vers la page d'accueil
                TempData["error"] = $"Une erreur s'est produite lors de la confirmation de votre commande: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> OrderDetail(Guid orderId)
        {
            try
            {
                // Récupérer les détails de la commande avec ses lignes
                var orderHeader = await _orderServices.GetOrderByIdAsync(orderId);
                
                if (orderHeader == null)
                {
                    TempData["error"] = "Commande introuvable";
                    return RedirectToAction(nameof(OrderIndex));
                }
                
                // Vérifier si l'utilisateur est autorisé à voir cette commande (admin ou propriétaire)
                var currentUser = await _signInManager.UserManager.GetUserAsync(User);
                if (!User.IsInRole("Admin") && currentUser != null && orderHeader.UserId != currentUser.Id)
                {
                    TempData["error"] = "Vous n'êtes pas autorisé à voir cette commande";
                    return RedirectToAction(nameof(OrderIndex));
                }
                
                // Mapper les données vers le ViewModel
                var orderViewModel = new Models.Order.OrderViewModel
                {
                    Id = orderHeader.Id,
                    OrderHeaderId = orderHeader.Id,  // Ajout de cette propriété pour correspondre à la vue
                    UserId = orderHeader.UserId,
                    CouponCode = orderHeader.CouponCode,
                    Discount = (decimal)(orderHeader.Discount),
                    OrderTotal = (decimal)(orderHeader.OrderTotal ?? 0),
                    Name = orderHeader.Name,
                    Phone = orderHeader.Phone,
                    Email = orderHeader.Email,
                    OrderTime = orderHeader.OrderTime,
                    Status = orderHeader.Status,
                    PaymentIntentId = orderHeader.PaymentIntentId,
                    StripeSessionId = orderHeader.StripeSessionId,
                    OrderDetails = orderHeader.OrderDetails.Select(od => new Models.Order.OrderDetailsViewModel
                    {
                        Id = od.Id,
                        OrderHeaderId = od.OrderHeaderId,
                        ProductId = od.ProductId,
                        ProductName = od.Product?.Name ?? "Produit inconnu",
                        Price = (decimal)od.Price,
                        Count = od.Count,
                        ImageUrl = od.Product?.ImageUrl
                    }).ToList()
                };
                
                return View(orderViewModel);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Erreur lors de la récupération des détails de la commande: {ex.Message}";
                return RedirectToAction(nameof(OrderIndex));
            }
        }
        
        [HttpGet]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProcessRefund(Guid orderHeaderId)
        {
            try
            {
                // Vérifier que la commande existe et a été payée
                var order = await _orderServices.GetOrderByIdAsync(orderHeaderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Commande introuvable" });
                }
                
                if (string.IsNullOrEmpty(order.PaymentIntentId))
                {
                    return Json(new { success = false, message = "Cette commande n'a pas de référence de paiement valide" });
                }
                
                // Traiter le remboursement via Stripe
                bool refundResult = await _orderServices.ProcessRefundAsync(orderHeaderId);
                
                if (refundResult)
                {
                    return Json(new { success = true, message = "Remboursement traité avec succès" });
                }
                else
                {
                    return Json(new { success = false, message = "Échec du remboursement" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            try
            {
                // Retrive the user from SinInManager
                ApplicationUser user = await _signInManager.UserManager.GetUserAsync(User);

                IEnumerable<OrderHeaderDto> list;

                if (User.IsInRole("Admin"))
                {
                    // Get all orders based on user role
                    list = _orderServices.GetAllOrdersAsync();
                }
                else
                {
                    list = _orderServices.GetOrdersByUserIdAsync(user.Id);
                }

                if (list != null)
                {
                    // Filter based on status if provided
                    switch (status?.ToLower())
                    {
                        case "approved":
                            list = list.Where(u => u.Status == StaticDetails.Status_Approved);
                            break;
                        case "readyforpickup":
                            list = list.Where(u => u.Status == StaticDetails.Status_ReadyForPickup);
                            break;
                        case "cancelled":
                            list = list.Where(u => u.Status == StaticDetails.Status_Cancelled ||
                                                 u.Status == StaticDetails.Status_Refunded);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    list = Enumerable.Empty<OrderHeaderDto>();
                }

                return Json(new { data = list.OrderByDescending(u => u.Id) });
            }
            catch (Exception ex)
            {
                // Return empty list in case of error
                return Json(new { data = Enumerable.Empty<OrderHeaderDto>() });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OrderReadyForPickup(Guid orderId)
        {
            try
            {
                var order = await _orderServices.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    TempData["error"] = "Order not found";
                    return RedirectToAction(nameof(OrderDetail), new { orderId });
                }

                var response = await _orderServices.UpdateOrderStatusAsync(orderId, StaticDetails.Status_ReadyForPickup);
                if (response.HasValue && response.Value)
                {
                    TempData["success"] = "Status updated successfully";
                    return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while updating the order status";
                return RedirectToAction(nameof(OrderDetail), new { orderId });
            }
        }

        [HttpPost("CompleteOrder")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CompleteOrder(Guid orderId)
        {
            try
            {
                var order = await _orderServices.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    TempData["error"] = "Order not found";
                    return RedirectToAction(nameof(OrderDetail), new { orderId });
                }

                var response = await _orderServices.UpdateOrderStatusAsync(orderId, StaticDetails.Status_Completed);
                if (response.HasValue && response.Value)
                {
                    TempData["success"] = "Status updated successfully";
                    return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while updating the order status";
                return RedirectToAction(nameof(OrderDetail), new { orderId });
            }
        }


        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            try
            {
                var order = await _orderServices.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    TempData["error"] = "Order not found";
                    return RedirectToAction(nameof(OrderDetail), new { orderId });
                }

                // Only allow cancellation of approved orders
                if (order.Status == StaticDetails.Status_Approved)
                {
                    if (!string.IsNullOrEmpty(order.PaymentIntentId))
                    {
                        var refundResult = await _stripePayment.StripeRefundOptions(order.PaymentIntentId);
                        if (!refundResult)
                        {
                            TempData["error"] = "Failed to process refund";
                            return RedirectToAction(nameof(OrderDetail), new { orderId });
                        }
                    }

                    var response = await _orderServices.UpdateOrderStatusAsync(orderId, StaticDetails.Status_Cancelled);
                    if (response != null)
                    {
                        TempData["success"] = "Order cancelled and refunded successfully";
                    }
                    else
                    {
                        TempData["error"] = "Failed to update order status";
                    }
                }
                else
                {
                    TempData["error"] = "Order cannot be cancelled in its current status";
                }

                return RedirectToAction(nameof(OrderDetail), new { orderId });
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while cancelling the order";
                return RedirectToAction(nameof(OrderDetail), new { orderId });
            }
        }
    }
}
