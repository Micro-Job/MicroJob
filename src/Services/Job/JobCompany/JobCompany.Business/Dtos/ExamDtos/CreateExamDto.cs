using JobCompany.Business.Dtos.QuestionDtos;
using Microsoft.AspNetCore.Http;

namespace JobCompany.Business.Dtos.ExamDtos
{
    public record CreateExamDto
    {
        public IFormFile LogoUrl { get; set; }
        public Guid? TemplateId { get; set; }
        public string IntroDescription { get; set; }
        public string LastDescription { get; set; }
        public string Result { get; set; }
        public ICollection<QuestionCreateDto> Questions { get; set; }
    }
}