namespace JobCompany.Business.Dtos.AnswerDtos
{
    public record CreateAnswerDto
    {
        public string? Text { get; set; }
        public bool? IsCorrect { get; set; }
    }
}