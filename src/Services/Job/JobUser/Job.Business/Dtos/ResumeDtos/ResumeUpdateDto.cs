using FluentValidation;
using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Validators;
using Job.Core.Entities;
using Job.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Job.Business.Dtos.ResumeDtos
{
    public record ResumeUpdateDto
    {
        public string FatherName { get; set; }
        public string Position { get; set; }
        public IFormFile? UserPhoto { get; set; }
        public bool IsDriver { get; set; }
        public bool IsMarried { get; set; }
        public bool IsCitizen { get; set; }
        public Gender Gender { get; set; }
        public string? Adress { get; set; }
        public DateTime BirthDay { get; set; }
        public ICollection<EducationUpdateDto>? Educations { get; set; }
        public ICollection<LanguageUpdateDto>? Languages { get; set; }
        public ICollection<ExperienceUpdateDto>? Experiences { get; set; }
        public ICollection<CertificateUpdateDto>? Certificates { get; set; }
        public ICollection<NumberUpdateDto>? PhoneNumbers { get; set; }
    }

    public class ResumeUpdateDtoValidator : AbstractValidator<ResumeUpdateDto>
    {
        public ResumeUpdateDtoValidator()
        {
            RuleFor(x => x.FatherName)
                .NotEmpty().WithMessage("Father's name is required.")
                .MaximumLength(50).WithMessage("Father's name cannot exceed 50 characters.");

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("Position is required.")
                .MaximumLength(100).WithMessage("Position cannot exceed 100 characters.");

            When(x => x.UserPhoto != null, () =>
            {
                RuleFor(x => x.UserPhoto)
                    .SetValidator(new FileValidator())
                    .WithMessage("User Photo must be a valid image file.");
            });
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