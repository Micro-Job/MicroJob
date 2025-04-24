using FluentValidation;
using Job.Core.Enums;
using SharedLibrary.Helpers;

namespace Job.Business.Dtos.LanguageDtos
{
    public record LanguageUpdateDto
    {
        public string? Id { get; set; }
        public Language LanguageName { get; set; }
        public LanguageLevel LanguageLevel { get; set; }
    }

    public class LanguageUpdateDtoValidator : AbstractValidator<LanguageUpdateDto>
    {
        public LanguageUpdateDtoValidator()
        {
            RuleFor(x => x.LanguageName)
                .IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));
            RuleFor(x => x.LanguageLevel)
                .IsInEnum().WithMessage(MessageHelper.GetMessage("INVALID_FORMAT"));
        }
    }
}