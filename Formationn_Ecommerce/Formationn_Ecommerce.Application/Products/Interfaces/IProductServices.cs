using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formationn_Ecommerce.Application.Coupons.Dtos;
using Formationn_Ecommerce.Application.Products.Dtos;

namespace Formationn_Ecommerce.Application.Products.Interfaces
{
    public interface IProductServices
    {
        Task<ProductDto?> AddAsync(CreateProductDto createProductDto);
        Task<ProductDto> ReadByIdAsync(Guid productId);
        Task<IEnumerable<ProductDto>> ReadAllAsync();
        Task<IEnumerable<ProductDto>>? ReadProductsByCategoryName(string categoryName);
        Task UpdateAsync(UpdateProductDto updateProductDto);
        Task DeleteAsync(Guid id);
    }
}
