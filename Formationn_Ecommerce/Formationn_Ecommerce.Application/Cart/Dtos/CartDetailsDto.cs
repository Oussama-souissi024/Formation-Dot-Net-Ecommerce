using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formationn_Ecommerce.Application.Products.Dtos;

namespace Formationn_Ecommerce.Application.Cart.Dtos
{
    public class CartDetailsDto
    {
        public Guid Id { get; set; }
        public Guid CartHeaderId { get; set; }
        public Guid ProductId { get; set; }
        public int Count { get; set; }

        // Navigation property for UI display purposes
        public ProductDto? Product { get; set; }

        // Additional calculated properties for UI
        public decimal? Price { get; set; }
    }
}
