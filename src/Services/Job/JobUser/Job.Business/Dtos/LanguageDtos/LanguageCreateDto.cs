using FluentValidation;
using SharedLibrary.Helpers;

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
                .IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));
            RuleFor(x => x.LanguageLevel)
                .IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));
        }
    }
}