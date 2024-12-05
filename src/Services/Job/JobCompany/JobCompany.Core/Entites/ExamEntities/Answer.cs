using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites.ExamEntities
{
    public class Answer : BaseEntity
    {
        public Question Question { get; set; }
        public Guid QuestionId { get; set; }
        public string? Text { get; set; }
        public bool? IsCorrect { get; set; }
    }
}