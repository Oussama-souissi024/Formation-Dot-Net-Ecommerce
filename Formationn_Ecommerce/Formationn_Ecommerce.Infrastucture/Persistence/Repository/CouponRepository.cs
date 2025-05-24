using Formationn_Ecommerce.Core.Entities.Coupon;
using Formationn_Ecommerce.Core.Interfaces.Repositories;
using Formationn_Ecommerce.Core.Interfaces.Repositories.Base;
using Formationn_Ecommerce.Infrastucture.Persistence.Repository.Base;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formationn_Ecommerce.Infrastucture.Persistence.Repository
{
    public class CouponRepository : Repository<Core.Entities.Coupon.Coupon>, ICouponRepository
    {
        private readonly ApplicationDbContext _Context;
        public CouponRepository(ApplicationDbContext context) : base(context)
        {
            _Context = context;
        }

        public async Task<Core.Entities.Coupon.Coupon> AddAsync(Core.Entities.Coupon.Coupon coupon)
        {
            try
            {
                // Ajouter le coupon à la base de données
                await _context.Coupons.AddAsync(coupon);
                await _context.SaveChangesAsync();
                return coupon;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'ajout du coupon: {ex.Message}", ex);
            }
        }

        public async Task<Core.Entities.Coupon.Coupon> ReadByIdAsync(Guid couponId)
        {
            try
            {
                // Rechercher un coupon par son Id
                return await _context.Coupons.FirstOrDefaultAsync(c => c.Id == couponId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la lecture du coupon: {ex.Message}", ex);
            }
        }

        public async Task<Core.Entities.Coupon.Coupon> ReadByCouponCodeAsync(string couponCode)
        {
            try
            {
                // Rechercher un coupon par son code
                return await _context.Coupons.FirstOrDefaultAsync(c => c.CouponCode.ToLower() == couponCode.ToLower());
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la lecture du coupon par code: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Core.Entities.Coupon.Coupon>> ReadAllAsync()
        {
            try
            {
                // Récupérer tous les coupons
                return await _context.Coupons.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des coupons: {ex.Message}", ex);
            }
        }

        public async Task UpdateAsync(Core.Entities.Coupon.Coupon coupon)
        {
            try
            {
                // Mettre à jour le coupon dans la base de données
                _context.Coupons.Update(coupon);
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
                // Rechercher le coupon à supprimer
                var coupon = await _context.Coupons.FindAsync(id);
                if (coupon != null)
                {
                    // Supprimer le coupon
                    _context.Coupons.Remove(coupon);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression du coupon: {ex.Message}", ex);
            }
        }

        // Implémentation de l'interface IRepository

        public async Task Remove(Core.Entities.Coupon.Coupon entity)
        {
            try
            {
                // Supprimer l'entité
                _context.Coupons.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression: {ex.Message}", ex);
            }
        }

        public async Task Update(Core.Entities.Coupon.Coupon entity)
        {
            try
            {
                // Mettre à jour l'entité
                _context.Coupons.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour: {ex.Message}", ex);
            }
        }

        async Task<IEnumerable<Core.Entities.Coupon.Coupon>> IRepository<Core.Entities.Coupon.Coupon>.GetAllAsync()
        {
            // Réutiliser la méthode existante
            return await ReadAllAsync();
        }

        async Task<Core.Entities.Coupon.Coupon> IRepository<Core.Entities.Coupon.Coupon>.GetByIdAsync(Guid id)
        {
            // Réutiliser la méthode existante
            return await ReadByIdAsync(id);
        }
    }
}
