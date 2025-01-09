using SharedLibrary.Dtos.QuestionDtos;

namespace SharedLibrary.Responses;

public class GetExamQuestionsResponse
{
    public decimal LimitRate { get; set; }
    public byte? Duration { get; set; }
    public List<QuestionDetailDto> Questions { get; set; }
}
