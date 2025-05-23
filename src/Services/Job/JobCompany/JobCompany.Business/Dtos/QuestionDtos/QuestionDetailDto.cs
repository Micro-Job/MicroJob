using JobCompany.Business.Dtos.AnswerDtos;
using JobCompany.Core.Enums;

namespace JobCompany.Business.Dtos.QuestionDtos
{
    public record QuestionDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Image { get; set; }
        public QuestionType QuestionType { get; set; }
        public bool IsRequired { get; set; }
        public List<AnswerDetailDto>? Answers { get; set; }
    }

    public record GetQuestionByStepDto
    {
        public int CurrentStep { get; set; } 
        public int TotalSteps { get; set; } 
        public QuestionDetailDto? Question { get; set; }
    }
}