using FluentValidation;
using Job.Core.Enums;

namespace Job.Business.Dtos.EducationDtos
{
    public record EducationCreateDto
    {
        public string ResumeId { get; set; }
        public string InstitutionName { get; set; }
        public string Profession { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentEducation { get; set; }
        public ProfessionDegree ProfessionDegree { get; set; }
    }

    public class EducationCreateDtoValidator : AbstractValidator<EducationCreateDto>
    {
        public EducationCreateDtoValidator()
        {
            RuleFor(dto => dto.ResumeId)
                .NotEmpty().WithMessage("Resume ID cannot be empty.");
            RuleFor(dto => dto.InstitutionName)
                .NotEmpty().WithMessage("Institution Name cannot be empty.")
                .Length(2, 100).WithMessage("Institution Name must be between 2 and 100 characters.");
            RuleFor(dto => dto.Profession)
                .NotEmpty().WithMessage("Profession cannot be empty.")
                .Length(2, 50).WithMessage("Profession must be between 2 and 50 characters.");
            RuleFor(dto => dto.StartDate)
                .NotEmpty().WithMessage("Start Date cannot be empty.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Start Date cannot be in the future.");
            RuleFor(dto => dto.EndDate)
                .Must((dto, endDate) => dto.IsCurrentEducation || endDate.HasValue)
                .WithMessage("End Date must be provided if the education is not current.")
                .GreaterThan(dto => dto.StartDate)
                .When(dto => dto.EndDate.HasValue)
                .WithMessage("End Date must be after Start Date.");
            RuleFor(dto => dto.ProfessionDegree)
                .IsInEnum().WithMessage("Profession Degree must be a valid enum value.");
        }
    }
}