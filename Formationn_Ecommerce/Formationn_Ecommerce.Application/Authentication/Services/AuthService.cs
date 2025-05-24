using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formationn_Ecommerce.Application.Authentication.Dtos;
using Formationn_Ecommerce.Application.Authentication.Interfaces;
using Formationn_Ecommerce.Core.Entities.Identity;
using Formationn_Ecommerce.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using Stripe;

namespace Formationn_Ecommerce.Application.Authentication.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(
            IAuthRepository authRepository,
            SignInManager<ApplicationUser> signInManager)
        {
            _authRepository = authRepository;
            _signInManager = signInManager;
        }

        public async Task<bool> AssingnRole(string email, string roleName)
        {
            // Vérifier si l'utilisateur existe
            var user = await _authRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return false;
            }

            // Vérifier si le rôle existe, sinon le créer
            if (!await _authRepository.RoleExistsAsync(roleName))
            {
                var roleResult = await _authRepository.CreateRoleAsync(roleName);
                if (!roleResult.Succeeded)
                {
                    return false;
                }
            }

            // Attribuer le rôle à l'utilisateur
            var result = await _authRepository.AddUserToRoleAsync(user, roleName);

            return result.Succeeded;
        }

        public async Task<ResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = await _authRepository.GetUserByEmailAsync(loginRequestDto.Email);

            if (user == null)
                return new ResponseDto { Error = "Utilisateur non trouvé" };

            var result = await _signInManager.PasswordSignInAsync(
                user,
                loginRequestDto.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (!result.Succeeded)
                return new ResponseDto { Error = "Authentification échouée" };

            return new ResponseDto { IsSuccess = true };
        }

        public async Task<bool> Logout()
        {
            try
            {
                await _authRepository.SignOutAsync();
                return true;
            }
            catch
            {
                return false;
            }
 
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            // vérifier si l'utilisateur existe déja 
            var existingUser = await _authRepository.GetUserByEmailAsync(registrationRequestDto.Email);
            if (existingUser != null)
            {
                return "Un utilisateur avec cet email existe déjà.";
            }

            // Créer un nouvel utilisateur
            var newUser = new ApplicationUser
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };

            // Ajouter l'utilisateur
            var result = await _authRepository.CreateUserAsync(newUser, registrationRequestDto.Password);
            if(!result.Succeeded )
            {
                // Concaténer les erreurs
                return string.Join(",", result.Errors.Select(e => e.Description));
            }

            string roleName = registrationRequestDto.Role ?? "Customer";
            // Vérifier si le rôle existe, sinon le créer
            if(!await _authRepository.RoleExistsAsync(roleName))
            {
                await _authRepository.CreateRoleAsync(roleName);
            }

            await _authRepository.AddUserToRoleAsync(newUser, roleName);

            return "Inscription réussie!";
        }

        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            var user = await _authRepository.GetUserByEmailAsync(email);
            if (user == null)
                return null;
            return user;
       
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null)
                return null;

            return user;

        }

        public async Task<string> GenerateEmailConfirmationToken(string userId)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null)
                return null;

            return await _authRepository.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<bool> ConfirmEmail(string userId, string token)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _authRepository.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }

        public async Task<string> GeneratePasswordResetToken(string userId)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null)
                return null;

            return await _authRepository.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<bool> ResetPassword(string userId, string token, string newPassword)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _authRepository.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

        public async  Task<bool?> CheckConfirmedEmail(string email)
        {
            var result = await _authRepository.CheckConfirmedEmail(email);
            if(result == null)
            {
                return null;
            }
            return result;
        }
    }
}
