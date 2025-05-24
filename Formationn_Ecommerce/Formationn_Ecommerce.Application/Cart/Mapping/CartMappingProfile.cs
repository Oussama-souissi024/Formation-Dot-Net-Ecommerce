using AutoMapper;
using Formationn_Ecommerce.Application.Cart.Dtos;
using Formationn_Ecommerce.Core.Entities.Cart;

namespace Formationn_Ecommerce.Application.Cart.Mapping
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            // Map CartHeader <-> CartHeaderDto
            CreateMap<CartHeader, CartHeaderDto>()
                .ReverseMap();

            // Map CartDetails <-> CartDetailsDto
            CreateMap<CartDetails, CartDetailsDto>()
                .ReverseMap();
        }
    }
}
