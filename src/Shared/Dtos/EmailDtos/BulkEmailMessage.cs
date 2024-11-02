namespace SharedLibrary.Dtos.EmailDtos
{
    public class BulkEmailMessage
    {
        public List<string> Emails { get; set; } = new List<string>();

        public string? Subject { get; set; }

        public string? Content { get; set; }
    }
}
