# Intégration du Service de Mailing au Contrôleur d'Authentification

Ce document décrit les étapes nécessaires pour intégrer le service d'envoi d'emails dans le contrôleur d'authentification (`AuthController.cs`) en utilisant le service `EmailSender.cs`.

## Étapes d'intégration

### 1. Injection du service IEmailSender dans AuthController

```csharp
private readonly IAuthService _authService;
private readonly IEmailSender _emailSender;

public AuthController(IAuthService authService, IEmailSender emailSender)
{
    _authService = authService;
    _emailSender = emailSender;
}
```

### 2. Modification de la méthode Register pour envoyer un email de confirmation

Dans la méthode `Register` du contrôleur `AuthController`, après une inscription réussie, ajoutez l'appel au service d'envoi d'email :

```csharp
[HttpPost]
public async Task<IActionResult> Register(RegistrationRequestDto registrationRequestDto)
{
    // Code existant pour la validation et l'inscription
    
    var result = await _authService.Register(registrationRequestDto);
    if (result == "Inscription réussie!")
    {
        // Récupérer les informations nécessaires pour l'email de confirmation
        // Ces informations devraient être retournées par le service d'authentification
        // Vous devrez modifier IAuthService et son implémentation pour retourner ces données
        var user = await _authService.GetUserByEmail(registrationRequestDto.Email);
        var token = await _authService.GenerateEmailConfirmationToken(user.Id);
        
        // Envoyer l'email de confirmation
        await _emailSender.SendEmailConfirmationAsync(registrationRequestDto.Email, token, user.Id);
        
        TempData["success"] = "Inscription réussie. Veuillez vérifier votre email pour confirmer votre compte.";
        return RedirectToAction("Index", "Home");
    }
    
    // Code existant pour gérer les erreurs
}
```

### 3. Ajout d'une action pour confirmer l'email

Ajoutez une nouvelle action dans le contrôleur pour gérer la confirmation d'email :

```csharp
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
        TempData["success"] = "Email confirmé avec succès. Vous pouvez maintenant vous connecter.";
        return RedirectToAction(nameof(Login));
    }
    
    TempData["error"] = "Échec de la confirmation de l'email";
    return RedirectToAction("Index", "Home");
}
```

### 4. Ajout de fonctionnalités de réinitialisation de mot de passe

#### a. Action pour demander une réinitialisation de mot de passe

```csharp
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
```

#### b. Action pour réinitialiser le mot de passe

```csharp
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
```

### 5. Ajout d'un email de bienvenue après la confirmation de l'email

Dans l'action `ConfirmEmail`, après une confirmation réussie, envoyez un email de bienvenue :

```csharp
[HttpGet]
public async Task<IActionResult> ConfirmEmail(string token, string userId)
{
    // Code existant pour la confirmation de l'email
    
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
    
    // Code existant pour gérer les erreurs
}
```

### 6. Compléter l'implémentation des méthodes dans EmailSender

Pour que les fonctionnalités ci-dessus fonctionnent correctement, vous devez compléter l'implémentation des méthodes suivantes dans la classe `EmailSender` :

1. `SendEmailConfirmationAsync` (corriger l'implémentation existante pour qu'elle renvoie la tâche)
2. `SendPasswordResetEmailAsync` (implémenter cette méthode)
3. `SendWelcomeEmailAsync` (implémenter cette méthode)

Exemple pour `SendEmailConfirmationAsync` (correction) :

```csharp
public async Task SendEmailConfirmationAsync(string email, string confirmationToken, string userId)
{
    var encodedToken = HttpUtility.UrlEncode(confirmationToken);
    var confirmationLink = $"https://localhost:7229/api/EmailActions/confirm-page?token={encodedToken}&userId={userId}";
    var message = new EmailMessage
    {
        To = email,
        Subject = "Confirmation de votre adresse email Formation Test SMTP",
        Body = $@"
            <html>
            <body style=""font-family: Arial, sans-serif; line-height: 1.6;"">
                <div style=""max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e4e4e4; border-radius: 5px;"">

                    <p>Merci <h1 style=""color: #333; text-align: center;"">
                        Confirmation de votre adresse email
                    </h1>de vous être inscrit sur Nexus!</p>
                    <p>Cliquez sur le bouton ci-dessous pour confirmer votre adresse email:</p>
                    <div style=""text-align: center; margin: 30px 0;"">
                        <a href='{confirmationLink}' style=""display: inline-block; background-color: #4CAF50; color: white; padding: 12px 20px; text-decoration: none; border-radius: 4px;"">
                            Confirmer mon adresse email
                        </a>
                    </div>
                    <p>Ce lien expirera dans 24 heures.</p>
                    <div style=""margin-top: 30px; padding-top: 20px; border-top: 1px solid #e4e4e4;"">
                        <p>Cordialement,<br>L'équipe Nexus</p>
                    </div>
                </div>
            </body>
            </html>",
        IsHtml = true
    };
    
    // Envoyer l'email
    await SendEmailAsync(message);
}
```

## Configuration du service dans Program.cs

Assurez-vous que le service IEmailSender est correctement configuré dans Program.cs :

```csharp
// Configuration du service d'envoi d'emails
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailSender, EmailSender>();
```

## Configuration des paramètres SMTP dans appsettings.json

```json
"EmailSettings": {
  "SenderName": "Votre Application",
  "SenderEmail": "votre-email@exemple.com",
  "SmtpServer": "smtp.votre-serveur.com",
  "SmtpPort": 587,
  "EnableSsl": true,
  "SmtpUsername": "votre-username",
  "SmtpPassword": "votre-mot-de-passe"
}
```

## Conclusion

En suivant ces étapes, vous intégrerez avec succès le service d'envoi d'emails dans le contrôleur d'authentification, ce qui permettra :
1. L'envoi d'emails de confirmation lors de l'inscription
2. L'envoi d'emails de bienvenue après la confirmation
3. La gestion de la réinitialisation de mot de passe via email

N'oubliez pas de créer les vues nécessaires pour les nouvelles actions (ForgotPassword, ResetPassword) et de mettre à jour les interfaces de service (IAuthService) pour prendre en charge les nouvelles méthodes.
