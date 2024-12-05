namespace SharedLibrary.Dtos.EmailDtos
{
    public record BulkMessageWithCredentials
    {
        public BulkEmailMessage Message { get; set; }
        public SmtpSettings Settings { get; set; }
    }
}