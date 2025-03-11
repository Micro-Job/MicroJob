using FluentValidation;
using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Core.Enums;
using Microsoft.AspNetCore.Http;
using Shared.Enums;
using SharedLibrary.Enums;

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
                .NotEmpty().WithMessage("Father's name is required.")
                .MaximumLength(50).WithMessage("Father's name cannot exceed 50 characters.");

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("Position is required.")
                .MaximumLength(100).WithMessage("Position cannot exceed 100 characters.");

            RuleFor(x => x.Adress)
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");

            RuleFor(x => x.BirthDay)
                .NotEmpty().WithMessage("Birthday is required.")
                .LessThan(DateTime.Now).WithMessage("Birthday must be in the past.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Gender must be a valid option.");
        }
    }
}