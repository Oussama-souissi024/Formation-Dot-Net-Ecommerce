using System.ComponentModel.DataAnnotations;

namespace Formationn_Ecommerce.Models.Category
{
    public class DeleteCategoryViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Category Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
