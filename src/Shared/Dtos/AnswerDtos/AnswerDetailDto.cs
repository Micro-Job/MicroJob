namespace SharedLibrary.Dtos.AnswerDtos;

public class AnswerDetailDto
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
    public bool? IsCorrect { get; set; }
}
