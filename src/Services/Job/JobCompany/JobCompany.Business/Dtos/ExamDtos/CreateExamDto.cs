using FluentValidation;
using JobCompany.Business.Dtos.QuestionDtos;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Dtos.ExamDtos;

public record CreateExamDto
{
    public string? Title { get; set; }
    public string? IntroDescription { get; set; }
    public float LimitRate { get; set; }
    public ICollection<QuestionCreateDto>? Questions { get; set; }
    public bool IsTemplate { get; set; }
    public short? Duration { get; set; }
}

public class CreateExamDtoValidator : AbstractValidator<CreateExamDto>
{
    public CreateExamDtoValidator()
    {
        RuleFor(x => x.IntroDescription)
            .MaximumLength(4096)
            .WithMessage(MessageHelper.GetMessage("DESCRIPTION_MUST_NOT_EXCEED_4096_CHAR"))
            .When(x => !string.IsNullOrWhiteSpace(x.IntroDescription));

        RuleFor(x => x.LimitRate)
            .InclusiveBetween(0f, 100f)
            .WithMessage(MessageHelper.GetMessage("LIMIT_RATE_MUST_BE_BETWEEN_0_100"));

        RuleFor(x => x.Duration)
            .GreaterThan((short)0)
            .WithMessage(MessageHelper.GetMessage("DURATION_MUST_BE_GREATER_THAN_ZERO"))
            .When(x => x.Duration.HasValue);
    }
}
