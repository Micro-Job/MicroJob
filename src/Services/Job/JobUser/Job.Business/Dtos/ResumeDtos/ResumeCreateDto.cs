using FluentValidation;
using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Validators;
using Job.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Job.Business.Dtos.ResumeDtos
{
    public record ResumeCreateDto
    {
        public string UserId { get; set; }
        public string FatherName { get; set; }
        public string Position { get; set; }
        public IFormFile? UserPhoto { get; set; }
        public bool IsDriver { get; set; }
        public bool IsMarried { get; set; }
        public bool IsCitizen { get; set; }
        public bool IsMainNumber { get; set; }
        public bool IsMainEmail { get; set; }
        public string? ResumeEmail { get; set; }
        public Gender Gender { get; set; }
        public string? Adress { get; set; }
        public DateTime BirthDay { get; set; }
        public ICollection<NumberCreateDto>? PhoneNumbers { get; set; }
        public ICollection<EducationCreateDto> Educations {  get; set; } 
        public ICollection<ExperienceCreateDto> Experiences {  get; set; } 
        public ICollection<LanguageCreateDto> Languages {  get; set; } 
        public ICollection<CertificateCreateDto> Certificates {  get; set; } 
    }

    public class ResumeCreateDtoValidator : AbstractValidator<ResumeCreateDto>
    {
        public ResumeCreateDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

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