using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formationn_Ecommerce.Core.Entities.Coupon;
using Formationn_Ecommerce.Core.Not_Mapped_Entities;
using Formationn_Ecommerce.Entities.Orders;
using Formationn_Ecommerce.Infrastucture.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using Formationn_Ecommercee.Core.Interfaces.External;

namespace Formationn_Ecommerce.Infrastucture.External.Payment
{
    public class StripePayment : IStripePayment
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;

        public StripePayment(IConfiguration configuration, ApplicationDbContext db)
        {
            _configuration = configuration;
            _db = db;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        public async Task<string> AddStripeCoupon(Core.Entities.Coupon.Coupon couponEntity)
        {
            try
            {
                var options = new CouponCreateOptions
                {
                    Duration = "once",
                    AmountOff = (long)(couponEntity.DiscountAmount * 100),
                    Name = couponEntity.CouponCode,
                    Currency = "usd",
                    Id = couponEntity.CouponCode,
                };

                var service = new CouponService();
                var stripeCoupon = await service.CreateAsync(options);

                if (stripeCoupon != null && stripeCoupon.Valid)
                {
                    return stripeCoupon.Id;
                }
                return "Failed to create coupon";
            }
            catch (StripeException ex)
            {
                return $"Stripe Error: {ex.StripeError.Message}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> DeleteStripeCoupon(Core.Entities.Coupon.Coupon couponEntity)
        {
            try
            {
                var service = new CouponService();
                await service.DeleteAsync(couponEntity.CouponCode);
                return "Coupon deleted successfully";
            }
            catch (StripeException ex)
            {
                return $"Stripe Error: {ex.StripeError.Message}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<Core.Not_Mapped_Entities.StripeRequest?> CreateStripeSessionAsync(Core.Not_Mapped_Entities.StripeRequest stripeRequest)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequest.ApprovedUrl,
                    CancelUrl = stripeRequest.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    PaymentMethodTypes = new List<string> { "card" },
                };


                // Si un code promo est fourni, on tente de trouver l'ID Stripe du PromotionCode correspondant.
                if (!string.IsNullOrEmpty(stripeRequest.OrderHeader.CouponCode))
                {
                    var promoService = new PromotionCodeService();
                    var promoCodes = await promoService.ListAsync(new PromotionCodeListOptions
                    {
                        Code = stripeRequest.OrderHeader.CouponCode,
                        Limit = 1
                    });

                    if (promoCodes?.Data?.Any() == true)
                    {
                        options.Discounts = new List<SessionDiscountOptions>
                        {
                            new SessionDiscountOptions
                            {
                                PromotionCode = promoCodes.Data[0].Id
                            }
                        };
                    }
                    else
                    {
                        // Fallback : utiliser la valeur comme ID de coupon si aucun promotion code trouvé
                        options.Discounts = new List<SessionDiscountOptions>
                        {
                            new SessionDiscountOptions
                            {
                                Coupon = stripeRequest.OrderHeader.CouponCode
                            }
                        };
                    }
                }

                // Utiliser directement les détails de commande de la requête pour créer les items Stripe
                foreach (var item in stripeRequest.OrderDetails ?? stripeRequest.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                                Description = $"Quantity: {item.Count}"
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                stripeRequest.StripeSessionUrl = session.Url;
                stripeRequest.StripeSessionId = session.Id;

                var orderHeader = await _db.OrderHeaders.FirstOrDefaultAsync(o => o.Id == stripeRequest.OrderHeader.Id);
                if (orderHeader != null)
                {
                    orderHeader.StripeSessionId = session.Id;
                    await _db.SaveChangesAsync();
                }

                return stripeRequest;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in CreateStripeSessionAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<OrderHeader> ValidateStripeSession(Guid orderHeaderId)
        {
            try
            {
                var orderHeader = await _db.OrderHeaders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.Id == orderHeaderId);

                if (orderHeader == null)
                {
                    throw new Exception($"Order header with ID {orderHeaderId} not found");
                }

                var sessionService = new SessionService();
                var session = await sessionService.GetAsync(orderHeader.StripeSessionId);

                if (session == null)
                {
                    throw new Exception($"Stripe session not found for ID: {orderHeader.StripeSessionId}");
                }

                if (session.PaymentStatus == "unpaid")
                {
                    orderHeader.Status = "Pending";
                    await _db.SaveChangesAsync();
                    return orderHeader;
                }

                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.GetAsync(session.PaymentIntentId);

                switch (paymentIntent.Status.ToLower())
                {
                    case "succeeded":
                        orderHeader.PaymentIntentId = paymentIntent.Id;
                        orderHeader.Status = "Approved";
                        await _db.SaveChangesAsync();
                        break;

                    case "requires_payment_method":
                        orderHeader.Status = "PaymentRequired";
                        await _db.SaveChangesAsync();
                        break;

                    case "requires_action":
                        orderHeader.Status = "PaymentPending";
                        await _db.SaveChangesAsync();
                        break;

                    default:
                        orderHeader.Status = "PaymentFailed";
                        await _db.SaveChangesAsync();
                        break;
                }

                return orderHeader;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error validating payment: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> StripeRefundOptions(string paymentIntentId)
        {
            try
            {
                var options = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntentId,
                    Reason = RefundReasons.RequestedByCustomer
                };

                var service = new RefundService();
                var refund = await service.CreateAsync(options);

                return refund.Status == "succeeded";
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
