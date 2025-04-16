using AuthService.Core.Entities;
using SharedLibrary.Dtos.EmailDtos;

namespace AuthService.Business.HelperServices.Email
{
    public interface IEmailService
    {
        Task SendSetPassword(string toEmail, string token);
        Task SendResetPassword(User user, string token);
        Task SendEmailAsync(string toEmail, EmailMessage emailMessage);
    }
}