using System.ComponentModel.DataAnnotations;

namespace Formationn_Ecommerce.Models.Category
{
    public class CategoryViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The Category name is required.")]
        [StringLength(100, ErrorMessage = "The Category name cannot exceed 100 characters.")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "The description cannot exceed 500 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
