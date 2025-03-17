using FluentValidation;
using SharedLibrary.Helpers;

namespace Job.Business.Dtos.ExperienceDtos
{
    public record ExperienceUpdateDto
    {
        public string Id { get; set; }
        public string OrganizationName { get; set; }
        public string PositionName { get; set; }
        public string? PositionDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentOrganization { get; set; }
    }

    public class ExperienceUpdateDtoValidator : AbstractValidator<ExperienceUpdateDto>
    {
        public ExperienceUpdateDtoValidator()
        {
            RuleFor(dto => dto.OrganizationName)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .Length(1, 100).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_100"));

            RuleFor(dto => dto.PositionName)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .Length(1, 50).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_50"));

            RuleFor(dto => dto.StartDate)
                .NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage(MessageHelper.GetMessage("START_DATE_CANNOT_BE_IN_THE_FUTURE"));

            RuleFor(dto => dto.EndDate)
                .Must((dto, endDate) => dto.IsCurrentOrganization || endDate.HasValue)
                .WithMessage(MessageHelper.GetMessage("END_DATE_MUST_BE"))
                .GreaterThan(dto => dto.StartDate)
                .When(dto => dto.EndDate.HasValue)
                .WithMessage(MessageHelper.GetMessage("STARTDATE_MUST_BE_EARLIER_ENDATE"));

            RuleFor(dto => dto.PositionDescription)
                .MaximumLength(500).WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_500"));
        }
    }
}