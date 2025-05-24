using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Formationn_Ecommerce.Application.Products.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int Count { get; set; } = 1;
        [Display(Name = "Catégorie")]
        public string CategoryName { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
