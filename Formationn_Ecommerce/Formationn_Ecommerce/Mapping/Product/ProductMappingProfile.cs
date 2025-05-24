using AutoMapper;
using Formationn_Ecommerce.Application.Products.Dtos;
using Formationn_Ecommerce.Models.Product;

namespace Formationn_Ecommerce.Mapping.Product
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            // Mappings bidirectionnels entre DTO et ViewModel
            CreateMap<ProductDto, ProductViewModel>()
                //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                //.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty))
                //.ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                //.ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName ?? string.Empty))
                .ReverseMap();

            CreateMap<ProductDto, DeleteProductViewModel>();

            CreateMap<CreateProductViewModel, CreateProductDto>();

            // Mapping from ProductDto to UpdateProductViewModel for edit operations
            CreateMap<ProductDto, UpdateProductViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName ?? string.Empty))
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore()); // CategoryId is set separately in controller

            // Mapping from UpdateProductViewModel to UpdateProductDto with property name and type conversions
            CreateMap<UpdateProductViewModel, UpdateProductDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.HasValue ? src.Price.Value : 0))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count.HasValue ? src.Count.Value : 1))
                .ForMember(dest => dest.ImageFile, opt => opt.MapFrom(src => src.ImageFile))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));
        }
    }
}
