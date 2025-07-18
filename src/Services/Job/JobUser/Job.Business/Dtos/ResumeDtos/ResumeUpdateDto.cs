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
    public record ResumeUpdateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Position { get; set; } //Əgər position yoxdursa bu adda olan bir position yaradılır db-də
        public Guid? PositionId { get; set; } //Əgər position varsa db-də onun id-si yazılmalıdır
        public Guid? ParentPositionId { get; set; } 
        public IFormFile? UserPhoto { get; set; }
        public Driver IsDriver { get; set; }
        public FamilySituation IsMarried { get; set; }
        public Citizenship IsCitizen { get; set; }
        public Gender Gender { get; set; }
        public Military MilitarySituation { get; set; }
        public bool IsMainNumber { get; set; }
        public bool IsMainEmail { get; set; }
        public bool IsPublic { get; set; }
        public bool IsAnonym { get; set; }
        public string? ResumeEmail { get; set; }
        public string? Adress { get; set; }
        public DateTime BirthDay { get; set; }

        public ICollection<Guid>? SkillIds { get; set; }

        public ICollection<CertificateUpdateDto>? Certificates { get; set; }
        public ICollection<NumberUpdateDto> PhoneNumbers { get; set; }
        public ICollection<ExperienceUpdateDto>? Experiences { get; set; }
        public ICollection<EducationUpdateDto>? Educations { get; set; }
        public ICollection<LanguageUpdateDto>? Languages { get; set; }
    }

    public class ResumeUpdateDtoValidator : AbstractValidator<ResumeUpdateDto>
    {
        public ResumeUpdateDtoValidator()
        {
            //RuleFor(x => x.FatherName)
            //    .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
            //    .MaximumLength(50).WithMessage(x =>
            //MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.FatherName?.Length ?? 0, 50));

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .MaximumLength(100).WithMessage(x =>
                 MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.Position?.Length ?? 0, 100))
                .When(x => !string.IsNullOrEmpty(x.Position));

            RuleFor(x => x.BirthDay.Date.Year)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .LessThanOrEqualTo(DateTime.Now.Date.Year - 16).WithMessage(MessageHelper.GetMessage("BIRTHDAY_MUST_BE_IN_THE_PAST"));

            RuleFor(x => x.Adress)
                .MaximumLength(200).WithMessage(x =>
                 MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.Adress?.Length ?? 0, 200));

            RuleFor(x => x.BirthDay)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .LessThan(DateTime.Now).WithMessage(MessageHelper.GetMessage("BIRTHDAY_MUST_BE_IN_THE_PAST"));

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));
        }
    }
}