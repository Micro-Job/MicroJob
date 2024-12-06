namespace JobCompany.Business.Dtos.ExamDtos.AnswerDtos
{
    public record CreateAnswerDto
    {
        public Guid QuestionId { get; set; }
        public string? Text { get; set; }
        public bool? IsCorrect { get; set; }
    }
}