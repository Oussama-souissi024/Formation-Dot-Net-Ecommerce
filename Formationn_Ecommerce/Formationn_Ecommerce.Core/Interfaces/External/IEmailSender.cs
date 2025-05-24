using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Formationn_Ecommerce.Core.Not_Mapped_Entities;

namespace Formationn_Ecommerce.Core.Interfaces.External
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage emailMessage);
        Task SendWelcomeEmailAsync(string email, string username);
        Task SendPasswordResetEmailAsync(string email, string resetToken, string userId);
        Task SendEmailConfirmationAsync(string email, string confirmationToken, string userId);   
    }
}
