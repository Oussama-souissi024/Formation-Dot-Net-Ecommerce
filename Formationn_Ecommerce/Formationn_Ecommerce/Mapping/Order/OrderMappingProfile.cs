using AutoMapper;
using Formationn_Ecommerce.Application.Order.Dtos;
using Formationn_Ecommerce.Models.Order;

namespace Formationn_Ecommerce.Mapping.Order
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            // Mapping entre DTO et ViewModel de commande
            CreateMap<OrderHeaderDto, OrderViewModel>()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
                .ReverseMap();
                
            // Mapping entre DTO et ViewModel de détails de commande
            CreateMap<OrderDetailsDto, OrderDetailsViewModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
