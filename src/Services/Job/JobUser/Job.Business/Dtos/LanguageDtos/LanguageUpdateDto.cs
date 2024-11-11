using FluentValidation;
using Job.Core.Enums;

namespace Job.Business.Dtos.LanguageDtos
{
    public record LanguageUpdateDto
    {
        public string Id { get; set; }
        public Language LanguageName { get; set; }
        public LanguageLevel LanguageLevel { get; set; }
    }

    public class LanguageUpdateDtoValidator : AbstractValidator<LanguageUpdateDto>
    {
        public LanguageUpdateDtoValidator()
        {
            RuleFor(x => x.LanguageName)
                .IsInEnum().WithMessage("Invalid language name.");
            RuleFor(x => x.LanguageLevel)
                .IsInEnum().WithMessage("Invalid language level.");
        }
    }
}