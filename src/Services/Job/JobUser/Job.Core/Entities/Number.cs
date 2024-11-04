namespace Job.Core.Entities
{
    public class Number : BaseEntity
    {
        public Guid PersonId { get; set; }
        public Person Person { get; set; }
        public string PhoneNumber { get; set; }
    }
}