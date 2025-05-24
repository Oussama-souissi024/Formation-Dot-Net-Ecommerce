using Formationn_Ecommerce.Core.Common;
using Formationn_Ecommerce.Core.Entities.Cart;
using Formationn_Ecommerce.Core.Entities.CategoryE;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Formationn_Ecommerce.Core.Entities.Products
{
    // Classe qui représente un produit dans le système e-commerce
    public class Product : BaseEntity
    {
        // Nom du produit, champ obligatoire limité à 200 caractères
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        
        // Description détaillée du produit
        public string Description { get; set; }

        // Prix du produit avec 2 décimales, champ obligatoire
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }
        
        // URL de l'image du produit
        public string ImageUrl { get; set; }
        
        // Identifiant de la catégorie à laquelle appartient ce produit (clé étrangère)
        [Required]
        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }

        // Référence à l'objet Category associé (relation many-to-one)
        public Category Category { get; set; }

        // Collection des détails de panier où ce produit apparaît
        public ICollection<CartDetails> CartDetails { get; set; }
    }
}
