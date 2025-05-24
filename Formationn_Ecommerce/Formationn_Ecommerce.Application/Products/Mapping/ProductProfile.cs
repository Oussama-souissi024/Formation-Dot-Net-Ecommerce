using AutoMapper;
using Formationn_Ecommerce.Application.Coupons.Dtos;
using Formationn_Ecommerce.Application.Products.Dtos;
using Formationn_Ecommerce.Core.Entities.Products;

namespace Formationn_Ecommerce.Application.Products.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // Mapping de Product vers ProductDto
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.ImageFile, opt => opt.Ignore()) // Ignore l'upload dans l'autre sens
                .ReverseMap()
                .ForMember(dest => dest.Category, opt => opt.Ignore()) // Ignore Category pour éviter problème de navigation
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore()); // (optionnel) si besoin d'ignorer CategoryId aussi dans reverse

            // Mapping CreateProductDto -> Product
            CreateMap<CreateProductDto, Product>();

            // Mapping UpdateProductDto -> Product
            CreateMap<UpdateProductDto, Product>();
        }
    }

}
