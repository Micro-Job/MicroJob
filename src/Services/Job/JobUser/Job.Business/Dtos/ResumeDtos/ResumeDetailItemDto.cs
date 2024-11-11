using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.NumberDtos;
using Job.Core.Enums;

namespace Job.Business.Dtos.ResumeDtos
{
    public record ResumeDetailItemDto
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string Position { get; set; }
        public string? UserPhoto { get; set; }
        public bool IsDriver { get; set; }
        public bool IsMarried { get; set; }
        public bool IsCitizen { get; set; }
        public Gender Gender { get; set; }
        public string? Adress { get; set; }
        public DateTime BirthDay { get; set; }

        //public UserInformationDto User { get; set; }
        public string? ResumeEmail { get; set; }
        public ICollection<NumberGetByIdDto> PhoneNumbers { get; set; }
        public ICollection<EducationGetByIdDto> Educations { get; set; }
        public ICollection<ExperienceGetByIdDto>? Experiences { get; set; }
        public ICollection<LanguageGetByIdDto>? Languages { get; set; }
        public ICollection<CertificateGetByIdDto>? Certificates { get; set; }
    }
}