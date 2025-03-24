using JobCompany.Business.Dtos.QuestionDtos;

namespace JobCompany.Business.Dtos.ExamDtos;

public class GetExamQuestionsDetailDto
{
    public int TotalQuestions { get; set; }
    public decimal LimitRate { get; set; }
    public short? Duration { get; set; }
    public List<QuestionPublicDto> Questions { get; set; }
}
