using FluentValidation;
using Job.Core.Enums;
using SharedLibrary.Helpers;

namespace Job.Business.Dtos.EducationDtos
{
    public class EducationCreateDto
    {
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
            RuleFor(dto => dto.InstitutionName)
                .NotEmpty()
                .WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .Length(2, 100)
                .WithMessage(x =>
                 MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.InstitutionName?.Length ?? 0, 100));
            RuleFor(dto => dto.Profession)
                .NotEmpty()
                .WithMessage(MessageHelper.GetMessage("NOT_EMPTY"))
                .Length(2, 50)
                .WithMessage(x =>
                 MessageHelper.GetMessage("LENGTH_SIZE_EXCEEDED", x.Profession?.Length ?? 0, 50));
            RuleFor(dto => dto.StartDate)
                .NotEmpty()
                .WithMessage(MessageHelper.GetMessage("NOT_EMPTY"));

            RuleFor(dto => dto.EndDate)
                .GreaterThan(dto => dto.StartDate).WithMessage(MessageHelper.GetMessage("STARTDATE_MUST_BE_EARLIER_ENDATE"))
                .When(dto => dto.EndDate.HasValue);

            RuleFor(dto => dto.ProfessionDegree)
                .IsInEnum()
                .WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));
        }
    }
}
