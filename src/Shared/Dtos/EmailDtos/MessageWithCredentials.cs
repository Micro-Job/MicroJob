namespace SharedLibrary.Dtos.EmailDtos
{
    public record MessageWithCredentials
    {
        public EmailMessage Message { get; set; }
        public SmtpSettings Settings { get; set; }
    }
}
