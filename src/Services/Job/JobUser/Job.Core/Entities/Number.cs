namespace Job.Core.Entities
{
    public class Number : BaseEntity
    {
        public User User { get; set; }
        public Guid UserId { get; set; }
        public string PhoneNumber { get; set; }
    }
}