using FluentValidation;
using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.NumberDtos;
using Microsoft.AspNetCore.Http;
using Shared.Enums;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;

namespace Job.Business.Dtos.ResumeDtos
{
    public record ResumeCreateDto
    {
        public TestDto TestDto { get; set; }
        public ICollection<Guid>? SkillIds { get; set; }
        public ICollection<CertificateCreateDto>? Certificates { get; set; }

        public ICollection<NumberCreateDto> PhoneNumbers { get; set; }
        public ICollection<ExperienceCreateDto> Experiences { get; set; }
        public ICollection<EducationCreateDto> Educations { get; set; }
        public ICollection<LanguageCreateDto> Languages { get; set; }
    }

    public class TestDto
    {
        public string FatherName { get; set; }
        public string Position { get; set; }
        public IFormFile? UserPhoto { get; set; }
        public Driver IsDriver { get; set; }
        public FamilySituation IsMarried { get; set; }
        public Citizenship IsCitizen { get; set; }
        public Gender Gender { get; set; }
        public Military MilitarySituation { get; set; }
        public bool IsMainNumber { get; set; }
        public bool IsMainEmail { get; set; }
        public bool IsPublic { get; set; }
        public string? ResumeEmail { get; set; }
        public string? Adress { get; set; }
        public DateTime BirthDay { get; set; }
    }

    public class ResumeCreateDtoValidator : AbstractValidator<ResumeCreateDto>
    {
        public ResumeCreateDtoValidator()
        {
            RuleFor(x => x.TestDto.FatherName)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .MaximumLength(50).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_50"));

            RuleFor(x => x.TestDto.Position)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .MaximumLength(100).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_100"));

            RuleFor(x => x.TestDto.Adress)
                .MaximumLength(200).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_200"));

            RuleFor(x => x.TestDto.BirthDay)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .LessThan(DateTime.Now).WithMessage(MessageHelper.GetMessage("BIRTHDAY_MUST_BE_IN_THE_PAST"));

            RuleFor(x => x.TestDto.Gender)
                .IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));
        }
    }
}