using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formationn_Ecommerce.Core.Entities.CategoryE;
using Formationn_Ecommerce.Core.Interfaces.Repositories;
using Formationn_Ecommerce.Core.Interfaces.Repositories.Base;
using Formationn_Ecommerce.Infrastucture.Persistence.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Formationn_Ecommerce.Infrastucture.Persistence.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context): base(context)
        {
            _context = context;
        }

        public async Task<Category> AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await SaveChangesAsync();
            return category;
        }

        public async Task<Category> ReadByIdAsync (Guid categoryId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public async Task<IEnumerable<Category>> ReadAllAsync()
        {
            return await _context.Categories
                .ToListAsync();
        }

        public async Task<Guid?> GetCategoryIdByCategoryNameAsync (string categoryName)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryName.ToLower());
            return category?.Id;
        }

        public async Task Update (Category categoryToUpdate)
        {
            try
            {
                // Approche simple pour mettre à jour une catégorie
                _context.Categories.Update(categoryToUpdate);
                
                // Sauvegarder les changements dans la base de données
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                // Log l'exception ou la propager avec plus d'informations
                throw new Exception($"Erreur lors de la mise à jour de la catégorie: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(Guid categoryId)
        {
            try
            {
                // Trouver la catégorie à supprimer
                var category = await _context.Categories.FindAsync(categoryId);
                
                // Vérifier si la catégorie existe
                if (category != null)
                {
                    // Supprimer la catégorie
                    _context.Categories.Remove(category);
                    
                    // Sauvegarder les changements dans la base de données
                    await _context.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression de la catégorie: {ex.Message}", ex);
            }
        }

        // IRepository implementation (inherited methods we're overriding with specific implementations)
        Task<IEnumerable<Category>> IRepository<Category>.GetAllAsync()
        {
            return ReadAllAsync();
        }

        Task<Category> IRepository<Category>.GetByIdAsync(Guid id)
        {
            return ReadByIdAsync(id);
        }

        Task IRepository<Category>.Remove(Category entity)
        {
            _context.Categories.Remove(entity);
            return _context.SaveChangesAsync();
        }
    }
}
