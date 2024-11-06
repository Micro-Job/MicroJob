using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Job.Business.Dtos.LanguageDtos
{
    public record LanguageCreateDto
    {
        public string ResumeId { get; set; }
        public Core.Enums.Language LanguageName { get; set; }
        public Core.Enums.LanguageLevel LanguageLevel { get; set; }
    }

    public class LanguageCreateDtoValidator : AbstractValidator<LanguageCreateDto>
    {
        public LanguageCreateDtoValidator()
        {
            RuleFor(x => x.ResumeId)
                .NotEmpty().WithMessage("Resume ID cannot be empty.");
            RuleFor(x => x.LanguageName)
                .IsInEnum().WithMessage("Invalid language name.");
            RuleFor(x => x.LanguageLevel)
                .IsInEnum().WithMessage("Invalid language level.");
        }
    } 
}