using Job.Core.Enums;
using Shared.Enums;
using SharedLibrary.Enums;

namespace Job.Core.Entities
{
    public class Resume : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        //public string Position { get; set; }
        public string? UserPhoto { get; set; }
        public Driver IsDriver { get; set; }
        public FamilySituation IsMarried { get; set; }
        public Citizenship IsCitizen { get; set; }
        public Gender Gender { get; set; }
        public Military MilitarySituation { get; set; }
        public bool IsPublic { get; set; }
        public string? Adress { get; set; }
        public DateTime BirthDay { get; set; }
        public string? ResumeEmail { get; set; }

        public ICollection<Number> PhoneNumbers { get; set; }
        public ICollection<Education> Educations { get; set; }
        public ICollection<Experience>? Experiences { get; set; }
        public ICollection<Language>? Languages { get; set; }
        public ICollection<Certificate>? Certificates { get; set; }
        public ICollection<ResumeSkill> ResumeSkills { get; set; }
        public ICollection<SavedResume> SavedResumes { get; set; }
    }
}