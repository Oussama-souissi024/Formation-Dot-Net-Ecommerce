using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Formationn_Ecommerce.Application.Categories.Dtos;
using Formationn_Ecommerce.Core.Entities.CategoryE;

namespace Formationn_Ecommerce.Application.Categories.Mapping
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            // Mapping between entity and DTO
            CreateMap<Category, CategoryDto>().ReverseMap();

            // Mapping for creation operations
            CreateMap<CreateCategoryDto, Category>();

            // Mapping for update operations
            CreateMap<UpdateCategoryDto, Category>();
        }
    }
}
