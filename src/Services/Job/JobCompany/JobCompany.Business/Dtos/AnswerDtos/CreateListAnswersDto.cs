namespace JobCompany.Business.Dtos.AnswerDtos
{
    public class CreateListAnswersDto
    {
        public ICollection<CreateAnswerDto> Answers { get; set; }
    }
}