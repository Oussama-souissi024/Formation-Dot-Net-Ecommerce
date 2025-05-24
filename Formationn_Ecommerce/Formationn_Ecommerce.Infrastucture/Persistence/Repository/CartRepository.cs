using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formationn_Ecommerce.Core.Entities.Cart;
using Formationn_Ecommerce.Core.Interfaces.Repositories;
using Formationn_Ecommerce.Core.Interfaces.Repositories.Base;
using Formationn_Ecommerce.Infrastucture.Persistence.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Formationn_Ecommerce.Infrastucture.Persistence.Repository
{
    public class CartRepository : Repository<CartHeader>, ICartRepository 
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<CartHeader?> GetCartHeaderByUserIdAsync(string userId)
        {
            try
            {
                return await _context.CartHeaders
                                     .Include(c => c.CartDetails)
                                     .FirstOrDefaultAsync(h => h.UserID == userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public async Task<IEnumerable<CartDetails>> GetListCartDetailsByCartHeaderIdAsync(Guid cartHeaderId)
        {
            return await _context.CartDetails
                                 .Where(d => d.CartHeaderId == cartHeaderId)
                                 .ToListAsync();
        }

        public async Task<CartHeader> AddCartHeaderAsync(CartHeader cartHeader)
        {
            await _context.CartHeaders.AddAsync(cartHeader);
            await _context.SaveChangesAsync();
            return cartHeader;
        }
        public async Task<CartDetails> AddCartDetailsAsync(CartDetails cartDetails)
        {
            await _context.CartDetails.AddAsync(cartDetails);
            await _context.SaveChangesAsync();
            return cartDetails;
        }

        public async Task<CartDetails?> GetCartDetailsByCartHeaderIdAndProductId(Guid cartHeaderId, Guid productId)
        {
            return await _context.CartDetails
                                 .FirstOrDefaultAsync(d => d.CartHeaderId == cartHeaderId && d.ProductId == productId);
        }

        public async Task<CartHeader?> GetCartHeaderByCartHeaderId(Guid cartHeaderId)
        {
            return await _context.CartHeaders
                                 .Include(h => h.CartDetails)
                                 .FirstOrDefaultAsync(h => h.Id == cartHeaderId);
        }

        public async Task<CartDetails?> GetCartDetailsByCartDetailsId(Guid cartDetailsId)
        {
            return await _context.CartDetails
                                 .FirstOrDefaultAsync(d => d.Id == cartDetailsId);
        }

        public async Task<CartHeader?> UpdateCartHeaderAsync(CartHeader cartHeader)
        {
            _context.CartHeaders.Update(cartHeader);
            await _context.SaveChangesAsync();
            return cartHeader;
        }

        public async Task<CartDetails?> UpdateCartDetailsAsync(CartDetails cartDetails)
        {
            _context.CartDetails.Update(cartDetails);
            await _context.SaveChangesAsync();
            return cartDetails;
        }

        public async Task<CartHeader?> RemoveCartHeaderAsync(CartHeader cartHeader)
        {
            _context.CartHeaders.Remove(cartHeader);
            await _context.SaveChangesAsync();
            return cartHeader;
        }
        public async Task<CartDetails?> RemoveCartDetailsAsync(CartDetails cartDetails)
        {
            _context.CartDetails.Remove(cartDetails);
            await _context.SaveChangesAsync();
            return cartDetails;
        }

        public int TotalCountofCartItem(Guid cartHeaderId)
        {
            return _context.CartDetails
                           .Where(d => d.CartHeaderId == cartHeaderId)
                           .Sum(d => d.Count);
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            var cartHeader = await GetCartHeaderByUserIdAsync(userId);

            var details = cartHeader.CartDetails.ToList();

            if (cartHeader is null)
                return false;

            _context.CartDetails.RemoveRange(details);
            _context.CartHeaders.Remove(cartHeader);

            await _context.SaveChangesAsync();
            return true;
        }

        public Task<CartDetails> AddAsync(CartDetails entity)
        {
            throw new NotImplementedException();
        }

        Task<CartDetails> IRepository<CartDetails>.GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<CartDetails>> IRepository<CartDetails>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task Update(CartDetails entity)
        {
            throw new NotImplementedException();
        }

        public Task Remove(CartDetails entity)
        {
            throw new NotImplementedException();
        }
    }
}
