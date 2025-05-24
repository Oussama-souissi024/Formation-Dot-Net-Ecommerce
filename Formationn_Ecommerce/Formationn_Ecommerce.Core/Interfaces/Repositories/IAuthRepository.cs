using Formationn_Ecommerce.Core.Entities.Identity;
using Formationn_Ecommerce.Core.Interfaces.Repositories.Base;
using Microsoft.AspNetCore.Identity;

namespace Formationn_Ecommerce.Core.Interfaces.Repositories
{
    public interface IAuthRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<ApplicationUser?> GetUserByUsernameAsync(string username);
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
        Task<bool> RoleExistsAsync(string roleName);
        Task<IdentityResult> CreateRoleAsync(string roleName);
        Task<IdentityResult> AddUserToRoleAsync(ApplicationUser user, string roleName);
        Task<bool> SignOutAsync();
        
        // Méthodes pour la confirmation d'email
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token);
        
        // Méthodes pour la réinitialisation de mot de passe
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
        Task<bool?> CheckConfirmedEmail(string email);
    }
}
