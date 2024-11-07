using FluentValidation;

namespace Job.Business.Dtos.LanguageDtos
{
    public record LanguageCreateDto
    {
        public Core.Enums.Language LanguageName { get; set; }
        public Core.Enums.LanguageLevel LanguageLevel { get; set; }
    }

    public class LanguageCreateDtoValidator : AbstractValidator<LanguageCreateDto>
    {
        public LanguageCreateDtoValidator()
        {
            RuleFor(x => x.LanguageName)
                .IsInEnum().WithMessage("Invalid language name.");
            RuleFor(x => x.LanguageLevel)
                .IsInEnum().WithMessage("Invalid language level.");
        }
    }
}