using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formationn_Ecommerce.Application.Order.Dtos
{
    public class OrderHeaderDto
    {
        // Unique identifier for the order
        public Guid Id { get; set; }

        // ID of the user who placed the order
        public string? UserId { get; set; }

        // Coupon code applied to this order, if any
        public string? CouponCode { get; set; }

        // Amount discounted from the order total
        public decimal Discount { get; set; }

        // Final total amount of the order after discounts
        public decimal? OrderTotal { get; set; }

        // Customer's name for delivery/contact
        public string? Name { get; set; }

        // Customer's phone number for order updates
        public string? Phone { get; set; }

        // Customer's email for order confirmation
        public string? Email { get; set; }

        // Timestamp when the order was placed
        public DateTime OrderTime { get; set; }

        // Current status of the order (e.g., Pending, Approved)
        public string? Status { get; set; }

        // Stripe payment processing IDs
        // Used for payment tracking and refunds
        public string? PaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }

        // Collection of items in this order
        // Contains details of each product ordered
        public IEnumerable<OrderDetailsDto> OrderDetails { get; set; }
    }
}
