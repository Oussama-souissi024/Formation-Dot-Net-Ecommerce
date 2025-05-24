using AutoMapper;
using Formationn_Ecommerce.Application.Categories.Dtos;
using Formationn_Ecommerce.Application.Coupons.Dtos;
using Formationn_Ecommerce.Core.Entities.CategoryE;
using Formationn_Ecommerce.Models.Category;
using Formationn_Ecommerce.Models.Coupon;

namespace Formationn_Ecommerce.Mapping.Category
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            // Mapping pour CategoryViewModel <-> CategoryDto
            CreateMap<CategoryViewModel, CategoryDto>().ReverseMap();

            // Mapping pour CreateCategoryViewModel <-> CreateCategoryDto
            CreateMap<CreateCategoryViewModel, CreateCategoryDto>();
            CreateMap<CreateCategoryDto, CreateCategoryViewModel>();

            // Mapping pour DeleteCategoryViewModel <-> CategoryDto
            CreateMap<DeleteCategoryViewModel, CategoryDto>();
            CreateMap<CategoryDto, DeleteCategoryViewModel>();
            
            // Mapping pour UpdateCategoryDto
            CreateMap<CategoryViewModel, UpdateCategoryDto>();
            CreateMap<UpdateCategoryDto, CategoryViewModel>();
        }
    }
}
