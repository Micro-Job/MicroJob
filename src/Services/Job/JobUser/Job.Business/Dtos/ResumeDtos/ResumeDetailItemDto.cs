using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Dtos.SkillDtos;
using Job.Core.Enums;
using Shared.Enums;
using SharedLibrary.Enums;

namespace Job.Business.Dtos.ResumeDtos
{
    public record ResumeDetailItemDto
    {
        public Guid? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string Position { get; set; }
        public string? UserPhoto { get; set; }
        public Driver IsDriver { get; set; }
        public FamilySituation IsMarried { get; set; }
        public Citizenship IsCitizen { get; set; }
        public Gender Gender { get; set; }
        public Military MilitarySituation { get; set; }
        public string? Adress { get; set; }
        public DateTime BirthDay { get; set; }

        //public UserInformationDto User { get; set; }
        public string? ResumeEmail { get; set; }
        public ICollection<SkillGetByIdDto>? Skills { get; set; }
        public ICollection<NumberGetByIdDto>? PhoneNumbers { get; set; }
        public ICollection<EducationGetByIdDto>? Educations { get; set; }
        public ICollection<ExperienceGetByIdDto>? Experiences { get; set; }
        public ICollection<LanguageGetByIdDto>? Languages { get; set; }
        public ICollection<CertificateGetByIdDto>? Certificates { get; set; }
    }
}