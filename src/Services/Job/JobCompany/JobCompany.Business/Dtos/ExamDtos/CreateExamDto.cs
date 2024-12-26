using JobCompany.Business.Dtos.QuestionDtos;
using Microsoft.AspNetCore.Http;

namespace JobCompany.Business.Dtos.ExamDtos
{
    public record CreateExamDto
    {
        public string? IntroDescription { get; set; }
        public string? LastDescription { get; set; }
        public ICollection<QuestionCreateDto>? Questions { get; set; }
    }
}