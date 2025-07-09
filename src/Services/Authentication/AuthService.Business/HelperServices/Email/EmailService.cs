using AuthService.Business.Templates;
using AuthService.Core.Entities;
using Microsoft.Extensions.Options;
using SharedLibrary.Dtos.EmailDtos;
using SharedLibrary.Helpers;
using System.Net;
using System.Net.Mail;

namespace AuthService.Business.HelperServices.Email;

public class EmailService(IOptions<SmtpSettings> smtpSettings, EmailTemplate _emailTemplate) 

{
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

    public void SendSetPassword(string toEmail, string token)
    {
        SendEmail(toEmail, new EmailMessage
        {
            Subject = "Şifrənizi müəyyən edin...",
            Content = _emailTemplate.ResetPassword(toEmail, token)
        });
    }

    public void SendResetPassword(User user, string token)
    {
        SendEmail(user.Email, new EmailMessage
        {
            Subject = "Şifrənizi yeniləyin...",
            Content = _emailTemplate.ResetPassword(token, $"{user.FirstName} {user.LastName}")
        });
    }

    public void SendRegister(string toEmail, string fullName)
    {
        SendEmail(toEmail, new EmailMessage
        {
            Subject = "Xoş gəlmişsiniz...",
            Content = _emailTemplate.RegisterCompleted(fullName)
        });
    }

    public void SendVerifyEmail(string toEmail, string fullName, string userId)
    {
        SendEmail(toEmail, new EmailMessage
        {
            Subject = "Hesabınızı təsdiqləyin...",
            Content = _emailTemplate.VerifyEmail(fullName, userId)
        });
    }

    //TODO : bu daha sonra email update edilərsə istifadə ediləcək
    public void SendVerifyUpdateEmail(string toEmail, string fullName, string userId)
    {
        SendEmail(toEmail, new EmailMessage
        {
            Subject = "Hesabınızı təsdiqləyin...",
            Content = _emailTemplate.VerifyUpdateEmail(fullName, userId)
        });
    }

    public void SendEmail(string toEmail, EmailMessage emailMessage)
    {
        var fromAddress = new MailAddress(_smtpSettings.Username, "HIRI");
        var toAddress = new MailAddress(toEmail);

        var smtp = new SmtpClient
        {
            Host = _smtpSettings.Server,
            Port = _smtpSettings.Port,
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, _smtpSettings.Password)
        };

        using (var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = emailMessage.Subject,
            IsBodyHtml = true,
            Body = emailMessage.Content,
        })
        smtp.Send(message);
    }


}
