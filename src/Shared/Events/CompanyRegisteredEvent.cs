namespace SharedLibrary.Events
{
    public class CompanyRegisteredEvent
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public string? VOEN { get; set; }
        public string? CompanyName { get; set; }
        public bool IsCompany { get; set; }
        public string Email { get; set; }
        public string MainPhoneNumber { get; set; }
    }
}