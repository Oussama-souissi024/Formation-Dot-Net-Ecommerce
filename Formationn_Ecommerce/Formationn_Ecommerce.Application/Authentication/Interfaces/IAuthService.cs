using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formationn_Ecommerce.Application.Authentication.Dtos;
using Formationn_Ecommerce.Core.Entities.Identity;

namespace Formationn_Ecommerce.Application.Authentication.Interfaces
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<ResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> Logout();
        Task<bool> AssingnRole(string email, string roleName);
        
        // Méthodes pour la gestion des utilisateurs
        Task<ApplicationUser> GetUserByEmail(string email);
        Task<ApplicationUser> GetUserById(string userId);
        
        // Méthodes pour la confirmation d'email
        Task<string> GenerateEmailConfirmationToken(string userId);
        Task<bool> ConfirmEmail(string userId, string token);
        
        // Méthodes pour la réinitialisation de mot de passe
        Task<string> GeneratePasswordResetToken(string userId);
        Task<bool> ResetPassword(string userId, string token, string newPassword);

        Task<bool?> CheckConfirmedEmail(string email);
    }
}
