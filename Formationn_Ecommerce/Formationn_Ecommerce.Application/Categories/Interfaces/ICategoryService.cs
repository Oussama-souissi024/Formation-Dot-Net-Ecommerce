using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formationn_Ecommerce.Application.Categories.Dtos;

namespace Formationn_Ecommerce.Application.Categories.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto> AddAsync(CreateCategoryDto categoryDto);
        Task<CategoryDto> ReadByIdAsync(Guid categoryId);
        Task<Guid?> GetCategoryIdByNameAsync(string categoryName);
        Task<IEnumerable<CategoryDto>> ReadAllAsync();
        Task UpdateAsync(UpdateCategoryDto updateCategoryDto);
        Task DeleteAsync(Guid id);
    }
}
