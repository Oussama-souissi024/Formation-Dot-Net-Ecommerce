using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formationn_Ecommerce.Application.Products.Dtos;

namespace Formationn_Ecommerce.Application.Order.Dtos
{
    public class OrderDetailsDto
    {
        public Guid Id { get; set; }
        public Guid OrderHeaderId { get; set; }
        public Guid ProductId { get; set; }
        public ProductDto? Product { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}
