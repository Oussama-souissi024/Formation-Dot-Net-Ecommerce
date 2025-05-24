using Formationn_Ecommerce.Application.Authentication.Dtos;
using Formationn_Ecommerce.Application.Authentication.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Formationn_Ecommerce.Core.Interfaces.External;

namespace Formationn_Ecommerce.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IEmailSender _emailSender;

        public AuthController(IAuthService authService, IEmailSender emailSender)
        {
            _authService = authService;
            _emailSender = emailSender;
        }


        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text="Administrateur",Value="Admin"},
                new SelectListItem{Text="Client",Value="Customer"},
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto registrationRequestDto)
        {
            if (!ModelState.IsValid)
            {
                //Réafficher la liste des Roles en cas d'erreur
                var roleList = new List<SelectListItem>()
                {
                    new SelectListItem{Text="Administrateur", Value="Admin"},
                    new SelectListItem { Text = "Client", Value = "Customer" }
                };
                ViewBag.roleList = roleList;
                return View(registrationRequestDto);
            }

            var result = await _authService.Register(registrationRequestDto);
            if (result == "Inscription réussie!")
            {
                // Récupérer les informations nécessaires pour l'email de confirmation
                var user = await _authService.GetUserByEmail(registrationRequestDto.Email);
                var token = await _authService.GenerateEmailConfirmationToken(user.Id);
                
                // Envoyer l'email de confirmation
                await _emailSender.SendEmailConfirmationAsync(registrationRequestDto.Email, token, user.Id);
                
                TempData["success"] = "Inscription réussie. Veuillez vérifier votre email pour confirmer votre compte.";
                return RedirectToAction("Index", "Home");
            }

            TempData["error"] = "Erreur d'inscription";
            ModelState.AddModelError(string.Empty, result);

            // Réafficher la liste des rôles en cas d'erreur
            var roleListError = new List<SelectListItem>()
            {
                new SelectListItem{Text="Administrateur",Value="Admin"},
                new SelectListItem{Text="Client",Value="Customer"},
            };
            ViewBag.RoleList = roleListError;
            return View(registrationRequestDto);
        }


        [HttpGet]
        public IActionResult Login()
        {
            // Vérifier si l'utilisateur est déjà connecté
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginRequestDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginRequestDto);
            }

            var result = await _authService.CheckConfirmedEmail(loginRequestDto.Email);
            if(result == null)
            {
                TempData["error"] = "Identifiants invalides";
                ModelState.AddModelError(string.Empty, "Nom d'utilisateur ou mot de passe incorrect.");
                return View(loginRequestDto);
            }

            if(result == false)
            {
                TempData["error"] = "Votre email est pas encore confirmé";
                ModelState.AddModelError(string.Empty, "Votre email est pas encore confirmé.");
                return View(loginRequestDto);
            }

            var user = await _authService.Login(loginRequestDto);
            if (user.IsSuccess)
            {
                TempData["success"] = "Connexion réussie";
                return RedirectToAction("Index", "Home");
            }

            TempData["error"] = "Identifiants invalides";
            ModelState.AddModelError(string.Empty, "Nom d'utilisateur ou mot de passe incorrect.");
            return View(loginRequestDto);
        }


        public async Task<IActionResult> Logout()
        {
            var result = await _authService.Logout();
            if (result)
            {
                TempData["success"] = "Déconnexion réussie";
                return RedirectToAction(nameof(Login));
            }
            TempData["error"] = "Erreur lors de la déconnexion";
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            {
                TempData["error"] = "Lien de confirmation invalide";
                return RedirectToAction("Index", "Home");
            }
            
            var result = await _authService.ConfirmEmail(userId, token);
            if (result)
            {
                // Récupérer l'utilisateur pour obtenir son email et son nom d'utilisateur
                var user = await _authService.GetUserById(userId);
                
                // Envoyer un email de bienvenue
                await _emailSender.SendWelcomeEmailAsync(user.Email, user.UserName);
                
                TempData["success"] = "Email confirmé avec succès. Vous pouvez maintenant vous connecter.";
                return RedirectToAction(nameof(Login));
            }
            
            TempData["error"] = "Échec de la confirmation de l'email";
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "L'adresse email est requise");
                return View();
            }
            
            var user = await _authService.GetUserByEmail(email);
            if (user != null)
            {
                var token = await _authService.GeneratePasswordResetToken(user.Id);
                await _emailSender.SendPasswordResetEmailAsync(email, token, user.Id);
            }
            
            // Pour des raisons de sécurité, ne pas révéler si l'email existe ou non
            TempData["success"] = "Si votre email existe dans notre système, vous recevrez un lien de réinitialisation de mot de passe.";
            return RedirectToAction(nameof(Login));
        }



        [HttpGet]
        public IActionResult ResetPassword(string token, string userId)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            {
                TempData["error"] = "Lien de réinitialisation invalide";
                return RedirectToAction("Index", "Home");
            }
            
            var model = new ResetPasswordDto { Token = token, UserId = userId };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var result = await _authService.ResetPassword(model.UserId, model.Token, model.NewPassword);
            if (result)
            {
                TempData["success"] = "Mot de passe réinitialisé avec succès. Vous pouvez maintenant vous connecter avec votre nouveau mot de passe.";
                return RedirectToAction(nameof(Login));
            }
            
            TempData["error"] = "Échec de la réinitialisation du mot de passe";
            ModelState.AddModelError(string.Empty, "Lien de réinitialisation invalide ou expiré");
            return View(model);
        }
    }
}
