namespace JobCompany.Business.Dtos.AnswerDtos;

public class UpdateAnswerDto
{
    public Guid? Id { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
}
