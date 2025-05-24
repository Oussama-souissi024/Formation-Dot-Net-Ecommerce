using Formationn_Ecommerce.Core.Common;
using Formationn_Ecommerce.Core.Entities.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formationn_Ecommerce.Core.Entities.CategoryE
{
    // Classe qui représente une catégorie de produits dans le système e-commerce
    public class Category : BaseEntity
    {
        // Nom de la catégorie, requis et limité à 100 caractères
        [Required(ErrorMessage = "The Category name is required.")]
        [MaxLength(100, ErrorMessage = " The Category name cannot exceed 100 Characters .")]
        public string Name { get; set; }

        // Description de la catégorie, limitée à 500 caractères
        [MaxLength(500, ErrorMessage = "The description cannot exceed 500 characters.")]
        public string Description { get; set; }

        // Collection des produits appartenant à cette catégorie (relation one-to-many)
        public ICollection<Product> Products { get; set; }
    }
}
