using Formationn_Ecommerce.Core.Common;
using Formationn_Ecommerce.Core.Entities.Products;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Formationn_Ecommerce.Core.Entities.Cart
{
    // Classe qui représente les détails d'un article dans le panier d'achat
    public class CartDetails : BaseEntity
    {
        // Identifiant de l'en-tête du panier auquel appartient ce détail (clé étrangère)
        [Required]
        [ForeignKey("CartHeader")]
        public Guid CartHeaderId { get; set; }

        // Identifiant du produit ajouté au panier
        public Guid ProductId { get; set; }

        // Quantité du produit dans le panier, limitée entre 1 et 100
        [Required]
        [Range(1, 100, ErrorMessage = "The value must be between 1 and 100.")]
        public int Count { get; set; }

        // Référence à l'en-tête du panier associé (relation many-to-one)
        public CartHeader CartHeader { get; set; }

        // Référence au produit associé à cette ligne du panier (relation many-to-one)
        public Product Product { get; set; }
    }
}
