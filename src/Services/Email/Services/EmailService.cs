using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using SharedLibrary.Dtos.EmailDtos;

namespace EmailService.API.Services
{
    public class EmailService(IOptions<SmtpSettings> smtpSettings) : IEmailService
    {
        private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

        #region bir mail ile isleyen emeliyyatlar 
     
        /// <summary>
        /// default smtp deyerlerini goturur
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(EmailMessage model)
        {
            var mailMessage = CreateMailMessage(model,null,null);
            await SendAsync(mailMessage, _smtpSettings);
        }

        /// <summary>
        /// smtp deyerlerini kenardan goturur
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="customSmtpSettings"></param>
        /// <returns></returns>
        public async Task SendMessageWithCredentialsAsync(EmailMessage model, SmtpSettings customSmtpSettings)
        {
            var mailMessage = CreateMailMessage(model,customSmtpSettings);
            await SendAsync(mailMessage, customSmtpSettings);
        }

        /// <summary>
        /// default smtp deyerleri ile isleyir ve attachment gonderir 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="attachments"></param>
        /// <returns></returns>
        public async Task SendMessageWithAttachmentsAsync(EmailMessage model, List<Attachment> attachments)
        {
            var mailMessage = CreateMailMessage(model,null,attachments);
            await SendAsync(mailMessage, _smtpSettings);
        }

        /// <summary>
        /// smtp deyerlerini kenardan goturur ve attachment gonderir
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="attachments"></param>
        /// <param name="customSmtpSettings"></param>
        /// <returns></returns>
        public async Task SendMessageWithCredentialsAndAttachmentsAsync(EmailMessage model, List<Attachment> attachments, SmtpSettings customSmtpSettings)
        {
            var mailMessage = CreateMailMessage(model, customSmtpSettings, attachments);
            await SendAsync(mailMessage, customSmtpSettings);
        }

        #endregion

        #region bulk mail emeliyyatlari 

        /// <summary>
        /// default smtp deyerlerini goturur (bulk)
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendBulkMessageAsync(BulkEmailMessage model)
        {
            var mailMessage = CreateBulkMailMessage(model,null,null);
            await SendAsync(mailMessage, _smtpSettings);
        }

        /// <summary>
        /// smtp deyerlerini kenardan goturur (bulk)
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="customSmtpSettings"></param>
        /// <returns></returns>
        public async Task SendBulkMessageWithCredentialsAsync(BulkEmailMessage model, SmtpSettings customSmtpSettings)
        {
            var mailMessage = CreateBulkMailMessage(model,customSmtpSettings,null);
            await SendAsync(mailMessage, customSmtpSettings);
        }

        /// <summary>
        /// default smtp deyerleri ile isleyir ve attachment gonderir (bulk)
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="customSmtpSettings"></param>
        /// <returns></returns>
        public async Task SendBulkMessageWithAttachmentsAsync(BulkEmailMessage model, List<Attachment> attachments)
        {
            var mailMessage = CreateBulkMailMessage(model,null,attachments);
            await SendAsync(mailMessage, _smtpSettings);
        }

        /// <summary>
        /// smtp deyerlerini kenardan goturur ve attachment gonderir (bulk)
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="attachments"></param>
        /// <param name="customSmtpSettings"></param>
        /// <returns></returns>
        public async Task SendBulkMessageWithCredentialsAndAttachmentsAsync(BulkEmailMessage model, List<Attachment> attachments, SmtpSettings customSmtpSettings)
        {
            var mailMessage = CreateBulkMailMessage(model, customSmtpSettings, attachments);
            await SendAsync(mailMessage, customSmtpSettings);
        }

        #endregion


        private MailMessage CreateMailMessage(EmailMessage model , SmtpSettings customSmtpSettings = null, List<Attachment> attachments = null)
        {
            MailAddress from = new MailAddress(customSmtpSettings is null ? _smtpSettings.FromAddress : customSmtpSettings.FromAddress);

            MailAddress to = new MailAddress(model.Email);

            MailMessage mail = new MailMessage(from, to)
            {
                Subject = model.Subject,
                Body = model.Content,
                IsBodyHtml = true,
            };

            if (customSmtpSettings != null)
            {
                if (customSmtpSettings.CcEmail != null)
                {
                    foreach (var item in customSmtpSettings.CcEmail)
                    {
                        mail.CC.Add(new MailAddress(item));
                    }
                }

                if (customSmtpSettings.BccEmail != null)
                {
                    foreach (var item in customSmtpSettings.BccEmail)
                    {
                        mail.Bcc.Add(new MailAddress(item));
                    }
                }
            }
            
            if (attachments != null)
            {
                foreach (var item in attachments)
                {
                    mail.Attachments.Add(item);
                }
            }

            return mail;
        }

        private MailMessage CreateBulkMailMessage(BulkEmailMessage model, SmtpSettings customSmtpSettings = null, List<Attachment> attachments = null)
        {
            MailAddress from = new MailAddress(customSmtpSettings is null ? _smtpSettings.FromAddress : customSmtpSettings.FromAddress);

            MailMessage mail = new MailMessage()
            {
                From = from,
                Subject = model.Subject,
                Body = model.Content,
                IsBodyHtml = true
            };

            foreach (var emails in model.Emails)
            {
                mail.To.Add(new MailAddress(emails));
            }

            if (customSmtpSettings != null)
            {
                if (customSmtpSettings.CcEmail != null)
                {
                    foreach (var item in customSmtpSettings.CcEmail)
                    {
                        mail.CC.Add(new MailAddress(item));
                    }
                }

                if (customSmtpSettings.BccEmail != null)
                {
                    foreach(var item in customSmtpSettings.BccEmail)
                    {
                        mail.Bcc.Add(new MailAddress(item));
                    }
                }
            }

            if (attachments != null)
            {
                foreach (var item in attachments)
                {
                    mail.Attachments.Add(item);
                }
            }

            return mail;
        }

        private async Task SendAsync(MailMessage mail, SmtpSettings smtpSettings)
        {
            using (SmtpClient smtp = new SmtpClient(smtpSettings.Server, smtpSettings.Port))
            {
                smtp.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
                smtp.EnableSsl = true;

                await smtp.SendMailAsync(mail);
            }
        }
    }
}
