using System.ComponentModel.DataAnnotations;

namespace Formationn_Ecommerce.Models.Product
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Nom du produit")]
        public string Name { get; set; }

        [Display(Name = "Prix")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        [Display(Name = "Catégorie")]
        public string CategoryName { get; set; }

    }
}
