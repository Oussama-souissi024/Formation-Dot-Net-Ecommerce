namespace Formationn_Ecommerce.Core.Interfaces.Repositories.Base
{
    // Interface générique définissant les opérations CRUD de base pour tout type d'entité
    public interface IRepository<TEntity> where TEntity : class
    {
        // Ajoute une nouvelle entité à la base de données
        Task<TEntity> AddAsync(TEntity entity);
        
        // Récupère une entité par son identifiant unique
        Task<TEntity> GetByIdAsync(Guid id);

        // Récupère toutes les entités de ce type
        Task<IEnumerable<TEntity>> GetAllAsync();

        // Met à jour une entité existante
        Task Update(TEntity entity);

        // Supprime une entité de la base de données
        Task Remove(TEntity entity);

        // Sauvegarde les changements dans la base de données et retourne le nombre d'enregistrements affectés
        Task<int> SaveChangesAsync();
    }
}
