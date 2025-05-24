using AutoMapper;
using Formationn_Ecommerce.Application.Products.Dtos;
using Formationn_Ecommerce.Models.Home;

namespace Formationn_Ecommerce.Mapping.Home
{
    public class HomeMappingProfile : Profile
    {
        public HomeMappingProfile()
        {
            // Mapping from ProductDto to HomeProductViewModel for home page display
            CreateMap<ProductDto, HomeProductViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl ?? string.Empty))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName ?? string.Empty));

            // Reverse mapping if needed (HomeProductViewModel to ProductDto)
            CreateMap<HomeProductViewModel, ProductDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl ?? string.Empty))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName ?? string.Empty))
                .ForMember(dest => dest.Count, opt => opt.Ignore())
                .ForMember(dest => dest.ImageFile, opt => opt.Ignore());
        }
    }
}
