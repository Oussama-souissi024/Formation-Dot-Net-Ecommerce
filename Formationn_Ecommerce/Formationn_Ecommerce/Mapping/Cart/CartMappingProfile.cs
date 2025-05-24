using AutoMapper;
using Formationn_Ecommerce.Application.Cart.Dtos;
using Formationn_Ecommerce.Application.Order.Dtos;
using Formationn_Ecommerce.Models.Cart;

namespace Formationn_Ecommerce.Mapping.Cart
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            CreateMap<CartDto, CartIndexViewModel>().ReverseMap();

            CreateMap<CartHeaderDto, OrderHeaderDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<CartDetailsDto, OrderDetailsDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
