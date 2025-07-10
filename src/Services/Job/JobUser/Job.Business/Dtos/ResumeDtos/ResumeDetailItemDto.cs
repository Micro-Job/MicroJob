using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.LinkDtos;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Dtos.SkillDtos;
using Job.Core.Enums;
using Microsoft.AspNetCore.Authentication.Cookies;
using Shared.Enums;
using SharedLibrary.Enums;

namespace Job.Business.Dtos.ResumeDtos
{
    public record ResumeDetailItemDto
    {
        public Guid UserId { get; set; }
        public Guid ResumeId { get; set; }
        public bool IsSaved { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Position { get; set; }
        public string? UserPhoto { get; set; }
        public Driver IsDriver { get; set; }
        public FamilySituation IsMarried { get; set; }
        public Citizenship IsCitizen { get; set; }
        public Gender Gender { get; set; }
        public Military MilitarySituation { get; set; }
        public string? Adress { get; set; }
        public DateTime BirthDay { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? ParentPositionId { get; set; }
        public bool IsMainEmail { get; set; }
        public bool IsMainNumber { get; set; }
        public bool IsPublic { get; set; }
        public bool IsAnonym { get; set; }
        public bool HasAccess { get; set; }
        public string? ResumeEmail { get; set; }
        public string? Summary { get; set; }

        public ICollection<LinkDto>? Urls { get; set; }
        public ICollection<SkillGetByIdDto>? Skills { get; set; }
        public ICollection<NumberGetByIdDto>? PhoneNumbers { get; set; }
        public ICollection<EducationGetByIdDto>? Educations { get; set; }
        public ICollection<ExperienceGetByIdDto>? Experiences { get; set; }
        public ICollection<LanguageGetByIdDto>? Languages { get; set; }
        public ICollection<CertificateGetByIdDto>? Certificates { get; set; }
    }
}