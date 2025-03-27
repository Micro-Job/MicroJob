using JobCompany.Business.Dtos.QuestionDtos;
using JobCompany.Core.Entites;

namespace JobCompany.Business.Dtos.ExamDtos
{
    public record GetExamByIdDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsTemplate { get; set; }
        public string IntroDescription { get; set; }
        public short? Duration { get; set; }
        public float LimitRate { get; set; }
    }
}