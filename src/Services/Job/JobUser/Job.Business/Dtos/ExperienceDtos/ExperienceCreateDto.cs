using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Job.Business.Dtos.ExperienceDtos
{
    public record ExperienceCreateDto
    {
        public string ResumeId { get; set; }
        public string OrganizationName { get; set; }
        public string PositionName { get; set; }
        public string? PositionDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentOrganization { get; set; }
    }

    public class ExperienceCreateDtoValidator : AbstractValidator<ExperienceCreateDto>
    {
        public ExperienceCreateDtoValidator()
        {
            RuleFor(dto => dto.ResumeId)
                .NotEmpty().WithMessage("Resume ID cannot be empty.");

            RuleFor(dto => dto.OrganizationName)
                .NotEmpty().WithMessage("Organization Name cannot be empty.")
                .Length(2, 100).WithMessage("Organization Name must be between 2 and 100 characters.");

            RuleFor(dto => dto.PositionName)
                .NotEmpty().WithMessage("Position Name cannot be empty.")
                .Length(2, 50).WithMessage("Position Name must be between 2 and 50 characters.");

            RuleFor(dto => dto.StartDate)
                .NotEmpty().WithMessage("Start Date cannot be empty.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Start Date cannot be in the future.");

            RuleFor(dto => dto.EndDate)
                .Must((dto, endDate) => dto.IsCurrentOrganization || endDate.HasValue)
                .WithMessage("End Date must be provided if the organization is not current.")
                .GreaterThan(dto => dto.StartDate)
                .When(dto => dto.EndDate.HasValue)
                .WithMessage("End Date must be after Start Date.");

            RuleFor(dto => dto.PositionDescription)
                .MaximumLength(500).WithMessage("Position Description must be 500 characters or fewer.");
        }
    }
}