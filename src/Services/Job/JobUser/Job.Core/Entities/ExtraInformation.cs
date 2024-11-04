namespace Job.Core.Entities
{
    public class ExtraInformation : BaseEntity
    {
        public Resume Resume { get; set; }
        public Guid ResumeId { get; set; }
        public ICollection<Language>? Languages { get; set; }
        public ICollection<Certificate>? Certificates { get; set; }
    }
}