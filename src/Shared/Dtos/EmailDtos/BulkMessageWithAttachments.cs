using System.Net.Mail;

namespace SharedLibrary.Dtos.EmailDtos
{
    public record BulkMessageWithAttachments
    {
        public BulkEmailMessage Message { get; set; }
        public List<Attachment> Attachments { get; set; }
    }
}
