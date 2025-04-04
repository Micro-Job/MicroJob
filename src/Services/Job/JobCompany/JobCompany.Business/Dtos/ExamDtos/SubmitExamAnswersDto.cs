namespace JobCompany.Business.Dtos.ExamDtos;

public class SubmitExamAnswersDto
{
    public Guid ExamId { get; set; }
    public Guid VacancyId { get; set; }
    public List<UserAnswerDto> Answers { get; set; }
}
