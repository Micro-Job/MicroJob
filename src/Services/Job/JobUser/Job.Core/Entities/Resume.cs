using Job.Core.Enums;

namespace Job.Core.Entities
{
    public class Resume : BaseEntity
    {
        public User User { get; set; }
        public Guid UserId { get; set; }
        public string FatherName { get; set; }
        public string? UserPhoto { get; set; }
        public bool IsDriver { get; set; }
        public bool IsMarried { get; set; }
        public bool IsCitizen { get; set; }
        public Gender Gender { get; set; }
        public string? Adress { get; set; }
        public DateTime BirthDay { get; set; }
        public ICollection<Number> PhoneNumbers { get; set; }
        public ICollection<Education> Educations { get; set; }
        public ICollection<Experience>? Experiences { get; set; }
        public ICollection<ExtraInformation>? ExtraInformations { get; set; }
    }
}