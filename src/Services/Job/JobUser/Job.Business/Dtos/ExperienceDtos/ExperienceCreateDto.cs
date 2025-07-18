using FluentValidation;
using SharedLibrary.Helpers;

namespace Job.Business.Dtos.ExperienceDtos
{
    public record ExperienceCreateDto
    {
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
            RuleFor(dto => dto.OrganizationName)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .Length(1, 100).WithMessage(x =>
                 MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.OrganizationName?.Length ?? 0, 100));

            RuleFor(dto => dto.PositionName)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY")) 
                .Length(1, 50).WithMessage(x =>
                 MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.PositionName?.Length ?? 0, 50));

            RuleFor(dto => dto.StartDate)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"));

            RuleFor(dto => dto.EndDate)
                .Must((dto, endDate) => dto.IsCurrentOrganization || endDate.HasValue)
                .WithMessage(MessageHelper.GetMessage("END_DATE_MUST_BE"))
                .GreaterThan(dto => dto.StartDate)
                .When(dto => dto.EndDate.HasValue)
                .WithMessage(MessageHelper.GetMessage("STARTDATE_MUST_BE_EARLIER_ENDATE"));

            RuleFor(dto => dto.PositionDescription)
                .MaximumLength(500).WithMessage(x =>
                 MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.PositionDescription?.Length ?? 0, 500));
        }
    }
}