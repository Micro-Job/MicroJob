using JobCompany.Business.Dtos.AnswerDtos;
using JobCompany.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace JobCompany.Business.Dtos.QuestionDtos;

public class QuestionUpdateDto
{
    public Guid? Id { get; set; }
    public string Title { get; set; }
    public IFormFile? Image { get; set; }
    public QuestionType QuestionType { get; set; }
    public bool IsRequired { get; set; }
    public int Order { get; set; }
    public bool IsImageDeleted { get; set; } // Şəkil silinirsə true olacaq
    public ICollection<UpdateAnswerDto>? Answers { get; set; }
}
