namespace JobCompany.Business.Dtos.ExamDtos.AnswerDtos
{
    public class CreateListAnswersDto
    {
        public ICollection<CreateAnswerDto> Answers { get; set; }
    }
}