using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Formationn_Ecommerce.Application.Products.Dtos
{
    public class UpdateProductDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public int? Count { get; set; } = 1;
        public IFormFile? ImageFile { get; set; }
    }
}
