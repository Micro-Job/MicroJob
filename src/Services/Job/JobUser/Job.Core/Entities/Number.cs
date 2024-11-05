namespace Job.Core.Entities
{
    public class Number : BaseEntity
    {
        public Resume Resume { get; set; }
        public Guid ResumeId { get; set; }
        public string PhoneNumber { get; set; }
    }
}