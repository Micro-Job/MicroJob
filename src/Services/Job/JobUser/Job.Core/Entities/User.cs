namespace Job.Core.Entities
{
    public class User : BaseEntity
    {
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string Email { get; set; }
        public string MainPhoneNumber { get; set; }
        public DateTime UserRegistrationDate { get; set; }
        public string UserPassword { get; set; }
        public DateTime? UserTokenExpireDate { get; set; }
    }
}
