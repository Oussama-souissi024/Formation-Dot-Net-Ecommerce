using Formationn_Ecommerce.Core.Entities.Coupon;
using Formationn_Ecommerce.Core.Interfaces.Repositories.Base;

namespace Formationn_Ecommerce.Core.Interfaces.Repositories
{
    // Interface qui définit les opérations spécifiques pour les coupons de réduction, hérite de l'interface générique IRepository
    public interface ICouponRepository : IRepository<Coupon>
    {
        // Ajoute un nouveau coupon à la base de données
        Task<Coupon> AddAsync(Coupon coupon);
        
        // Récupère un coupon par son identifiant
        Task<Coupon> ReadByIdAsync(Guid couponId);
        
        // Récupère un coupon par son code de réduction
        Task<Coupon> ReadByCouponCodeAsync(string couponCode);
        
        // Récupère tous les coupons existants
        Task<IEnumerable<Coupon>> ReadAllAsync();
        
        // Met à jour un coupon existant
        Task UpdateAsync(Coupon coupon);
        
        // Supprime un coupon par son identifiant
        Task DeleteAsync(Guid id);
    }
}
