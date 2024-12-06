using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using JobCompany.Core.Enums;

namespace JobCompany.Business.Dtos.QuestionDtos
{
    public record QuestionCreateDto
    {
        public string Title { get; set; }
        public string? Image { get; set; }
        public QuestionType QuestionType { get; set; }
        public bool IsRequired { get; set; }
    }

    public class QuestionCreateDtoValidator : AbstractValidator<QuestionCreateDto>
    {
        public QuestionCreateDtoValidator()
        {
            RuleFor(q => q.Title)
                .NotEmpty()
                .WithMessage("Sual başlığı boş ola bilməz.");

            When(q => q.QuestionType == QuestionType.ImageBased, () =>
            {
                RuleFor(q => q.Image)
                    .NotEmpty()
                    .WithMessage("Şəkil əsaslı suallar üçün şəkil URL-si tələb olunur.");
            });
        }
    }
}