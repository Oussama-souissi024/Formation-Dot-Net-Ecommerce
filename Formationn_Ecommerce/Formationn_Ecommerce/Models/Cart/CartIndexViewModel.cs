using Formationn_Ecommerce.Application.Cart.Dtos;
using System.Collections.Generic;

namespace Formationn_Ecommerce.Models.Cart
{
    public class CartIndexViewModel
    {
        public CartHeaderDto CartHeader { get; set; } = new CartHeaderDto();
        public List<CartDetailsDto> CartDetails { get; set; } = new List<CartDetailsDto>();
    }
}
