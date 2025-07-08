namespace JobCompany.Business.Dtos.AnswerDtos
{
    public record AnswerDetailDto
    {
        public Guid Id { get; set; }
        public string? Text { get; set; }
    }
}