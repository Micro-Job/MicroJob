using AuthService.DAL.Contexts;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using AuthService.Core.Entities;
using SharedLibrary.Exceptions;
using SharedLibrary.Dtos.EmailDtos;
using SharedLibrary.Helpers;

namespace AuthService.Business.HelperServices.Email
{
    public class EmailService(IOptions<SmtpSettings> smtpSettings,
                              AppDbContext _context) : IEmailService

    {
        private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

        public async Task SendSetPassword(string toEmail, string token)
        {
            //await SendEmailAsync( new EmailMessage
            //{
            //    Email = toEmail,
            //    Subject = "Şifrənizi müəyyən edin...",
            //    Content = EmailTemplate.ResetPassword(toEmail, token)
            //});
        }

        public async Task SendResetPassword(string toEmail, string token)
        {
            var user = await _context.PasswordTokens
                .Where(pt => pt.Token == token && pt.ExpireTime > DateTime.Now)
                .Select(pt => new
                {
                    //pt.User.UserName,
                    pt.User.Email
                })
                .SingleOrDefaultAsync();

            if (user == null || user.Email != toEmail) throw new NotFoundException<User>(MessageHelper.GetMessage("NOTFOUNDEXCEPTION_USER"));
            
            //await SendEmailAsync(new EmailMessage
            //{
            //    Email = toEmail,
            //    Subject = "Şifrənizi yeniləyin...",
            //    Content = EmailTemplate.ResetPassword(token, user.UserName)
            //});
        }       
    }
}
