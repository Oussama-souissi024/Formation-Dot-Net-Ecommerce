using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formationn_Ecommerce.Core.Entities.Products;
using Formationn_Ecommerce.Core.Interfaces.Repositories;
using Formationn_Ecommerce.Infrastucture.Persistence.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Formationn_Ecommerce.Infrastucture.Persistence.Repository
{
    public class ProductRepository : Repository<Product> , IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Product> AddAsync(Product  product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return (product);
        }

        public async Task<Product> ReadByIdAsync(Guid productId)
        {
            try
            {
                // Rechercher un coupon par son Id
                return await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la lecture du produit: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Product>> ReadAllAsync()
        {
            try
            {
                // Récupérer tous les coupons
                return await _context.Products.Include(p => p.Category).ToListAsync();// Use Include to fetch related categories
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des coupons: {ex.Message}", ex);
            }
        }

        public async Task UpdateAsync(Product product)
        {
            try
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour du coupon: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product != null)
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression du coupon: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryId(Guid categoryId)
        {
            return await _context.Products.Include(p => p.CategoryId == categoryId).ToListAsync();
        }
    }
}
