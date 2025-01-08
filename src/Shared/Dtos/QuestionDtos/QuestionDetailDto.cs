using SharedLibrary.Dtos.AnswerDtos;
using SharedLibrary.Enums;

namespace SharedLibrary.Dtos.QuestionDtos;

public class QuestionDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
    public QuestionType QuestionType { get; set; }
    public bool IsRequired { get; set; }
    public List<AnswerDetailDto> Answers { get; set; }
}