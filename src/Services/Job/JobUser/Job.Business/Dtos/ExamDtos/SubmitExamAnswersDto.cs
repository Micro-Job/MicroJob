namespace Job.Business.Dtos.ExamDtos;

public class SubmitExamAnswersDto
{
    public Guid ExamId { get; set; }
    public Guid UserId { get; set; }
    public List<UserAnswerDto> Answers { get; set; }
}
