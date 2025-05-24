using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formationn_Ecommerce.Core.Utility
{
    public static class StaticDetails
    {
        public const string Status_Pending = "Pending";        // Initial state when order is created
        public const string Status_Approved = "Approved";      // Order is approved after payment
        public const string Status_ReadyForPickup = "ReadyForPickup";  // Order is ready for customer pickup
        public const string Status_Completed = "Completed";    // Order has been delivered/picked up
        public const string Status_Refunded = "Refunded";      // Payment has been refunded
        public const string Status_Cancelled = "Cancelled";    // Order has been cancelled
    }
}
