using JobCompany.Business.Dtos.QuestionDtos;

namespace JobCompany.Business.Dtos.ExamDtos;

public class UpdateExamDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? IntroDescription { get; set; }
    public float LimitRate { get; set; }
    public ICollection<QuestionUpdateDto>? Questions { get; set; }
    public bool IsTemplate { get; set; }
    public short? Duration { get; set; }
}
