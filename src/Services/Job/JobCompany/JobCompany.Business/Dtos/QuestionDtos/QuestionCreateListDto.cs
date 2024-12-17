namespace JobCompany.Business.Dtos.QuestionDtos
{
    public class QuestionCreateListDto
    {
        public ICollection<QuestionCreateDto> Questions { get; set; }
    }
}