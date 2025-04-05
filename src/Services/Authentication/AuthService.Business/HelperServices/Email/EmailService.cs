using AuthService.DAL.Contexts;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using AuthService.Core.Entities;
using SharedLibrary.Exceptions;
using SharedLibrary.Dtos.EmailDtos;
using SharedLibrary.Helpers;
using AuthService.Business.Templates;
using System.Net.Mail;
using System.Net;

namespace AuthService.Business.HelperServices.Email
{
    public class EmailService(IOptions<SmtpSettings> smtpSettings,
                              AppDbContext _context, EmailTemplate _emailTemplate) : IEmailService

    {
        private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

        public async Task SendSetPassword(string toEmail, string token)
        {
            await SendEmailAsync(toEmail, new EmailMessage
            {
                Subject = "Şifrənizi müəyyən edin...",
                Content = _emailTemplate.ResetPassword(toEmail, token)
            });
        }

        public async Task SendEmailAsync(string toEmail, EmailMessage emailMessage)
        {
            var fromAddress = new MailAddress(_smtpSettings.Username, "Siesco");
            var toAddress = new MailAddress(toEmail);
            string fromPassword = _smtpSettings.Password;
            string subject = emailMessage.Subject;
            string body = emailMessage.Content;

            var smtp = new SmtpClient
            {
                Host = _smtpSettings.Server,
                Port = _smtpSettings.Port,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                IsBodyHtml = true,
                Body = body,
            })
                smtp.Send(message);
        }

        public async Task SendResetPassword(string toEmail, string token)
        {
            var user = await _context.PasswordTokens
                .Where(pt => pt.Token == token && pt.ExpireTime > DateTime.Now)
                .Select(pt => new
                {
                    Email = pt.User.Email,
                    UserName = pt.User.FirstName + pt.User.LastName
                })
                .SingleOrDefaultAsync();

            if (user == null || user.Email != toEmail) throw new NotFoundException<User>(MessageHelper.GetMessage("NOTFOUNDEXCEPTION_USER"));


            await SendEmailAsync(toEmail, new EmailMessage
            {
                Subject = "Şifrənizi yeniləyin...",
                Content = _emailTemplate.ResetPassword(token, user.UserName)
            });

        }
    }
}
