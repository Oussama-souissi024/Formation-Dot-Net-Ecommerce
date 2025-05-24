using Formationn_Ecommerce.Core.Entities.Products;
using Formationn_Ecommerce.Core.Interfaces.Repositories.Base;

namespace Formationn_Ecommerce.Core.Interfaces.Repositories
{
    // Interface qui définit les opérations spécifiques pour les produits, hérite de l'interface générique IRepository
    public interface IProductRepository : IRepository<Product>
    {
        // Ajoute un nouveau produit à la base de données
        Task<Product> AddAsync(Product product);
        
        // Récupère un produit par son identifiant
        Task<Product> ReadByIdAsync(Guid productId);
        
        // Récupère tous les produits existants
        Task<IEnumerable<Product>> ReadAllAsync();
        
        // Met à jour un produit existant
        Task UpdateAsync(Product product);
        
        // Supprime un produit par son identifiant et retourne true si l'opération a réussi
        Task DeleteAsync(Guid productId);

        Task<IEnumerable<Product>> GetProductsByCategoryId(Guid categoryId);
    }
}
