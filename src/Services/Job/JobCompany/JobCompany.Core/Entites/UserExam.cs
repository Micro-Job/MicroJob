using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites;

public class UserExam : BaseEntity
{
    public Guid UserId { get; set; }
    public Exam Exam { get; set; }
    public Guid ExamId { get; set; }
    public byte TrueAnswerCount { get; set; }
    public byte FalseAnswerCount { get; set; }
}
