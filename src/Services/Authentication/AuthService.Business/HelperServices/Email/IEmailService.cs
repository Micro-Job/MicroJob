using SharedLibrary.Dtos.EmailDtos;

namespace AuthService.Business.HelperServices.Email
{
    public interface IEmailService
    {
        Task SendSetPassword(string toEmail, string token);
        Task SendResetPassword(string toEmail, string token);
        Task SendEmailAsync(string toEmail, EmailMessage emailMessage);
    }
}