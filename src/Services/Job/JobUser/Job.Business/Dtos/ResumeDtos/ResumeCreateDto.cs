using FluentValidation;
using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.LinkDtos;
using Job.Business.Dtos.NumberDtos;
using Microsoft.AspNetCore.Http;
using Shared.Enums;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;

namespace Job.Business.Dtos.ResumeDtos
{
    public record ResumeCreateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid? PositionId { get; set; } //Əgər position varsa db-də onun id-si yazılmalıdır
        public string? Position { get; set; } //Əgər position yoxdursa bu adda olan bir position yaradılır db-də
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
        public string? Summary { get; set; }
        public DateTime BirthDay { get; set; }

        public ICollection<Guid>? SkillIds { get; set; }
        public ICollection<CertificateCreateDto>? Certificates { get; set; }
        public ICollection<CreateLinkDto>? Urls { get; set; }
        public ICollection<NumberCreateDto> PhoneNumbers { get; set; }
        public ICollection<ExperienceCreateDto>? Experiences { get; set; }
        public ICollection<EducationCreateDto>? Educations { get; set; }
        public ICollection<LanguageCreateDto>? Languages { get; set; }
    }

    public class ResumeCreateDtoValidator : AbstractValidator<ResumeCreateDto>
    {
        public ResumeCreateDtoValidator()
        {
            //RuleFor(x => x.FatherName)
            //    .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
            //    .MaximumLength(50).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_50"));

            RuleFor(x => x.Position)
                .MaximumLength(100).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_100"));

            RuleFor(x => x.Adress)
                .MaximumLength(200).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_200"));

            RuleFor(x => x.Summary)
                .MaximumLength(2048).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_2048"));

            //RuleFor(x => x.BirthDay)
            //    .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
            //    .LessThan(DateTime.Now).WithMessage(MessageHelper.GetMessage("BIRTHDAY_MUST_BE_IN_THE_PAST"));

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));

            RuleForEach(x => x.Certificates)
                .SetValidator(new CertificateCreateDtoValidator());

            RuleForEach(x => x.Experiences)
                .SetValidator(new ExperienceCreateDtoValidator());

            RuleForEach(x => x.Educations)
                .SetValidator(new EducationCreateDtoValidator());

            RuleForEach(x => x.Urls)
                .SetValidator(new CreateLinkValidator());
        }
    }
}