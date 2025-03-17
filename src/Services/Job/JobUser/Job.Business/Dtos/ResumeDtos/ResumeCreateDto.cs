using FluentValidation;
using Job.Business.Dtos.CertificateDtos;
using Microsoft.AspNetCore.Http;
using Shared.Enums;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;

namespace Job.Business.Dtos.ResumeDtos
{
    public record ResumeCreateDto
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
        public ICollection<Guid>? SkillIds { get; set; }
        public ICollection<CertificateCreateDto>? Certificates { get; set; }
    }

    public class ResumeCreateDtoValidator : AbstractValidator<ResumeCreateDto>
    {
        public ResumeCreateDtoValidator()
        {
            RuleFor(x => x.FatherName)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .MaximumLength(50).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_50"));

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .MaximumLength(100).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_100"));

            RuleFor(x => x.Adress)
                .MaximumLength(200).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_200"));

            RuleFor(x => x.BirthDay)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .LessThan(DateTime.Now).WithMessage(MessageHelper.GetMessage("BIRTHDAY_MUST_BE_IN_THE_PAST"));

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));
        }
    }
}