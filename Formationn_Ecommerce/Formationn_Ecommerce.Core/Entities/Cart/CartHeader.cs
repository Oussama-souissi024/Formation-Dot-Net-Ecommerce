using Formationn_Ecommerce.Core.Common;
using Formationn_Ecommerce.Core.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Formationn_Ecommerce.Core.Entities.Cart
{
    // Classe qui représente l'en-tête d'un panier d'achat, contenant les informations générales du panier
    public class CartHeader : BaseEntity
    {
        // Identifiant de l'utilisateur propriétaire du panier (clé étrangère)
        [Required]
        [ForeignKey("User")]
        public string UserID { get; set; }

        // Optional foreign key relationship - a cart may not have a coupon
        [ForeignKey("Coupon")]
        public Guid? CouponId { get; set; }

        // Code de coupon de réduction appliqué au panier
        public string? CouponCode { get; set; }

        // Référence à l'utilisateur propriétaire du panier (relation many-to-one)
        public ApplicationUser User { get; set; }

        // Référence à l'entité coupon
        public Formationn_Ecommerce.Core.Entities.Coupon.Coupon Coupon { get; set; }

        // Collection des détails du panier associés à cet en-tête (relation one-to-many)
        public ICollection<CartDetails> CartDetails { get; set; }
    }
}
