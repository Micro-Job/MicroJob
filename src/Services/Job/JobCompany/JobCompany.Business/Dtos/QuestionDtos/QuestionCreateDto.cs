using FluentValidation;
using JobCompany.Business.Dtos.AnswerDtos;
using JobCompany.Core.Enums;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Dtos.QuestionDtos
{
    public record QuestionCreateDto
    {
        public string Title { get; set; }
        public IFormFile? Image { get; set; }
        public QuestionType QuestionType { get; set; }
        public bool IsRequired { get; set; }
        public ICollection<CreateAnswerDto>? Answers { get; set; }
    }

    public class QuestionCreateDtoValidator : AbstractValidator<QuestionCreateDto>
    {
        public QuestionCreateDtoValidator()
        {
            RuleFor(q => q.Title).NotEmpty().WithMessage(MessageHelper.GetMessage("NOT_FOUND"));
        }
    }
}
