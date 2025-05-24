

using Formationn_Ecommerce.Core.Entities.Cart;
using Formationn_Ecommerce.Core.Interfaces.Repositories.Base;

namespace Formationn_Ecommerce.Core.Interfaces.Repositories
{
    public interface ICartRepository : IRepository<CartHeader>, IRepository<CartDetails>
    {
        Task<CartHeader?> GetCartHeaderByUserIdAsync(string userId);
        Task<IEnumerable<CartDetails>> GetListCartDetailsByCartHeaderIdAsync(Guid CartHeaderId);
        Task<CartHeader> AddCartHeaderAsync(CartHeader cartHeader);
        Task<CartDetails> AddCartDetailsAsync(CartDetails cartDetails);
        Task<CartDetails?> GetCartDetailsByCartHeaderIdAndProductId(Guid cartHeaderId, Guid productId);
        Task<CartHeader?> GetCartHeaderByCartHeaderId(Guid cartHeaderId);
        Task<CartDetails?> GetCartDetailsByCartDetailsId(Guid cartDetailsId);
        Task<CartHeader?> UpdateCartHeaderAsync(CartHeader cartHeader);
        Task<CartDetails?> UpdateCartDetailsAsync(CartDetails cartDetails);
        Task<CartHeader?> RemoveCartHeaderAsync(CartHeader cartHeader);
        Task<CartDetails?> RemoveCartDetailsAsync(CartDetails cartDetails);
        int TotalCountofCartItem(Guid cartHeaderId);
        Task<bool> ClearCartAsync(string userId);

    }
}
