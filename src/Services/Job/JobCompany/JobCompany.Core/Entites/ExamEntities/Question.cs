using JobCompany.Core.Enums;

namespace JobCompany.Core.Entites.ExamEntities
{
    public class Question
    {
        public string Title { get; set; }
        public string? Image { get; set; }
        public QuestionType QuestionType { get; set; }
        public bool IsRequired { get; set; }
        public Exam Exam { get; set; }
        public Guid ExamId { get; set; }
        public TimeSpan Time { get; set; }
        public ICollection<Answer>? Answers { get; set; }
    }
}