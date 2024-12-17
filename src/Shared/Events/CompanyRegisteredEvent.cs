namespace SharedLibrary.Events
{
    public class CompanyRegisteredEvent
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyLogo { get; set; }
    }
}