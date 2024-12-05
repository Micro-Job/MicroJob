namespace SharedLibrary.Dtos.EmailDtos
{
    public class SmtpSettings
    {
        public string? FromName { get; set; }
        public string FromAddress { get; set; }
        public string ToEmail { get; set; }
        public List<string>? CcEmail { get; set; }
        public List<string>? BccEmail { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}