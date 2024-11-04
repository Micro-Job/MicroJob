namespace Job.Core.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MainPhoneNumber { get; set; }
        public DateTime UserRegistrationDate { get; set; }
        public string UserPassword { get; set; }
        public DateTime? UserTokenExpireDate { get; set; }
    }
}
