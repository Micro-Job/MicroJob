using SharedLibrary.Dtos.EmailDtos;
using System.Net.Mail;

namespace EmailService.API.Services
{
    public interface IEmailService
    {
        Task SendMessageAsync(EmailMessage model);
        Task SendMessageWithCredentialsAsync(EmailMessage model, SmtpSettings customSmtpSettings);
        Task SendMessageWithAttachmentsAsync(EmailMessage model, List<Attachment> attachments);
        Task SendMessageWithCredentialsAndAttachmentsAsync(EmailMessage model, List<Attachment> attachments, SmtpSettings customSmtpSettings);

        Task SendBulkMessageAsync(BulkEmailMessage model);
        Task SendBulkMessageWithCredentialsAsync(BulkEmailMessage model, SmtpSettings customSmtpSettings);
        Task SendBulkMessageWithAttachmentsAsync(BulkEmailMessage model, List<Attachment> attachments);
        Task SendBulkMessageWithCredentialsAndAttachmentsAsync(BulkEmailMessage model, List<Attachment> attachments, SmtpSettings customSmtpSettings);

    }
}
