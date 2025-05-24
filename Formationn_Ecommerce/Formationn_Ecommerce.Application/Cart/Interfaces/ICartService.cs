using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formationn_Ecommerce.Application.Cart.Dtos;

namespace Formationn_Ecommerce.Application.Cart.Interfaces
{
    public interface ICartService
    {
        Task<CartDto?> GetCartByUserIdAsync(string userId);
        Task<CartDto> UpsertCartAsync(CartDto cartDto); // Update or Insert
        Task<CartDto> ApplyCouponAsync(string userId, string couponCode);
        Task<bool> RemoveCartItemAsync(Guid cartDetailsId);
        Task<bool> ClearCartAsync(string userId);
    }
}
