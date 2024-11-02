using System.Net.Mail;

namespace SharedLibrary.Dtos.EmailDtos
{
    public record MessageWithCredentialsAndAttachment
    {
        public EmailMessage Message { get; set; }
        public List<Attachment> Attachments { get; set; }
        public SmtpSettings Settings { get; set; }
    }
}
