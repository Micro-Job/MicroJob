using FluentValidation;
using Job.Core.Enums;
using SharedLibrary.Helpers;

namespace Job.Business.Dtos.EducationDtos
{
    public record EducationUpdateDto
    {
        public string Id {  get; set; }
        public string InstitutionName { get; set; }
        public string Profession { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentEducation { get; set; }
        public ProfessionDegree ProfessionDegree { get; set; }
    }

    public class EducationUpdateDtoValidator : AbstractValidator<EducationUpdateDto>
    {
        public EducationUpdateDtoValidator()
        {
            RuleFor(dto => dto.InstitutionName)
                .NotEmpty()
                .WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .Length(2, 100)
                .WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_100"));
            RuleFor(dto => dto.Profession)
                .NotEmpty()
                .WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .Length(2, 50)
                .WithMessage(MessageHelper.GetMessage("LENGTH_MUST_BE_BETWEEN_1_50"));
            RuleFor(dto => dto.StartDate)
                .NotEmpty()
                .WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage(MessageHelper.GetMessage("START_DATE_CANNOT_BE_IN_THE_FUTURE"));
            RuleFor(dto => dto.EndDate)
                .Must((dto, endDate) => dto.IsCurrentEducation || endDate.HasValue)
                .WithMessage(MessageHelper.GetMessage("END_DATE_MUST_BE"))
                .When(dto => dto.EndDate.HasValue)
                .WithMessage(MessageHelper.GetMessage("STARTDATE_MUST_BE_EARL?ER_ENDATE"));
            RuleFor(dto => dto.ProfessionDegree)
                .IsInEnum()
                .WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));
        }
    }
}