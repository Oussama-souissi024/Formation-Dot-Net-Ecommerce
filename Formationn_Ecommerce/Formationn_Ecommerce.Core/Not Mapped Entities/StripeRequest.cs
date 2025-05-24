using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formationn_Ecommerce.Entities.Orders;

namespace Formationn_Ecommerce.Core.Not_Mapped_Entities
{
    public class StripeRequest
    {
        public string? StripeSessionUrl { get; set; }
        public string? StripeSessionId { get; set; }
        public string ApprovedUrl { get; set; }
        public string CancelUrl { get; set; }
        public OrderHeader OrderHeader { get; set; }
        // Ajout d'une propriété pour les détails de la commande
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}
