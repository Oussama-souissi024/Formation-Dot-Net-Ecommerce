using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Formationn_Ecommerce.Core.Common;
using Formationn_Ecommerce.Core.Entities.Products;

namespace Formationn_Ecommerce.Entities.Orders
{
    public class OrderDetails : BaseEntity
    {
        
        // Foreign key linking to the order header
        // Each order detail must belong to an order
        [Required]
        [ForeignKey("OrderHeader")]
        public Guid OrderHeaderId { get; set; }

        // Foreign key linking to the product
        // Identifies which product was ordered
        [Required]
        [ForeignKey("Product")]
        public Guid ProductId { get; set; }

        // Price of the product at the time of purchase
        // Stored with 2 decimal places
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        // Quantity of the product ordered
        public int Count { get; set; }

        // Navigation properties for related entities
        // Enables easy access to order header and product information
        public OrderHeader OrderHeader { get; set; }
        public Product Product { get; set; }
    }
}
