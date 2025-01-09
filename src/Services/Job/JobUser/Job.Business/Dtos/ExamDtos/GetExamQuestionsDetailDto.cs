using Job.Business.Dtos.QuestionDtos;

namespace Job.Business.Dtos.ExamDtos;

public class GetExamQuestionsDetailDto
{
    public int TotalQuestions { get; set; }
    public decimal LimitRate { get; set; }
    public byte? Duration { get; set; }
    public List<QuestionPublicDto> Questions { get; set; }
}
