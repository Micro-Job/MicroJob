using Job.Business.Dtos.AnswerDtos;
using SharedLibrary.Enums;

namespace Job.Business.Dtos.QuestionDtos;

public class QuestionPublicDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
    public QuestionType QuestionType { get; set; }
    public bool IsRequired { get; set; }
    public List<AnswerPublicDto> Answers { get; set; }
}