using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Job.Business.Dtos.LanguageDtos
{
    public record LanguageUpdateDto
    {
        public Core.Enums.Language LanguageName { get; set; }
        public Core.Enums.LanguageLevel LanguageLevel { get; set; }
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