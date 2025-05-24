using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Formationn_Ecommerce.Core.Common;
using Formationn_Ecommerce.Core.Entities.Identity;

namespace Formationn_Ecommerce.Entities.Orders
{
    // Classe qui représente l'en-tête d'une commande dans le système e-commerce
    public class OrderHeader : BaseEntity
    {
        // Identifiant de l'utilisateur qui a passé la commande (clé étrangère)
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        // Code de coupon de réduction optionnel appliqué à cette commande
        public string? CouponCode { get; set; }

        // Montant de la réduction appliquée à la commande (avec 2 décimales)
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Discount { get; set; }

        // Montant total final de la commande après réductions (avec 2 décimales)
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? OrderTotal { get; set; }

        // Informations de contact du client
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        // Date et heure auxquelles la commande a été passée
        [Required]
        public DateTime OrderTime { get; set; }

        // Statut actuel de la commande (ex: En attente, Approuvée, Expédiée)
        public string Status { get; set; }

        // Champs de traitement des paiements Stripe pour le suivi des transactions
        public string? PaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }

        // Référence à l'utilisateur qui a passé la commande (relation many-to-one)
        public ApplicationUser User { get; set; }

        // Collection des articles dans cette commande (relation one-to-many)
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
