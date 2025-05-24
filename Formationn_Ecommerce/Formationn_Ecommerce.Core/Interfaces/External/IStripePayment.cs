
using Formationn_Ecommerce.Core.Entities.Coupon;
using Formationn_Ecommerce.Core.Not_Mapped_Entities;
using Formationn_Ecommerce.Entities.Orders;

namespace Formationn_Ecommercee.Core.Interfaces.External
{
    public interface IStripePayment
    {
        // Creates a new Stripe checkout session
        // Initializes payment intent and returns session details
        // Returns null if session creation fails
        Task<StripeRequest?> CreateStripeSessionAsync(StripeRequest stripeRequest);

        // Validates a completed Stripe payment session
        // Updates order status based on payment result
        // Returns updated order header information
        Task<OrderHeader> ValidateStripeSession(Guid orderHeaderId);

        // Creates a new coupon in Stripe's system
        // Syncs local coupon with Stripe's coupon
        // Returns success/error message
        Task<string> AddStripeCoupon(Coupon coupon);

        // Removes a coupon from Stripe's system
        // Should be called when deleting local coupons
        // Returns success/error message
        Task<string> DeleteStripeCoupon(Coupon coupon);

        // Processes a refund for a completed payment
        // Used for order cancellations or returns
        // Returns true if refund is successful
        Task<bool> StripeRefundOptions(string paymentIntentId);
    }
}
