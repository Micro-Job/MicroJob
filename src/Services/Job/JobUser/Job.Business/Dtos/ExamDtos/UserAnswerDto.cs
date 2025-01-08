namespace Job.Business.Dtos.ExamDtos;

public class UserAnswerDto
{
    public Guid QuestionId { get; set; }
    public List<Guid>? AnswerIds { get; set; }
    public string? Text { get; set; }
}
