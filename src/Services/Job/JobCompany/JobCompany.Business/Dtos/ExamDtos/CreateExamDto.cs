using JobCompany.Business.Dtos.QuestionDtos;
using Microsoft.AspNetCore.Http;

namespace JobCompany.Business.Dtos.ExamDtos
{
    public record CreateExamDto
    {
        public string? Title { get; set; }
        public string? IntroDescription { get; set; }
        public float LimitRate { get; set; }
        public ICollection<QuestionCreateDto>? Questions { get; set; }
        public bool IsTemplate { get; set; }
        public short? Duration { get; set; }
    }
}
