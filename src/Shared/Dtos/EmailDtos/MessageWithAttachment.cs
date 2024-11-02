using System.Net.Mail;

namespace SharedLibrary.Dtos.EmailDtos
{
    public record MessageWithAttachment
    {
        public EmailMessage Message { get; set; }
        public List<Attachment> Attachments { get; set; }
    }
}
