using JobCompany.Core.Entites.Base;
using JobCompany.Core.Enums;

namespace JobCompany.Core.Entites
{
    public class Question : BaseEntity
    {
        public string Title { get; set; }
        public string? Image { get; set; }
        public QuestionType QuestionType { get; set; }
        public bool IsRequired { get; set; }
        public ICollection<Answer>? Answers { get; set; }
        public ICollection<ExamQuestion> ExamQuestions { get; set; }
    }
}