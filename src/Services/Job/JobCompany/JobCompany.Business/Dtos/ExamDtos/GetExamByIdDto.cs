using JobCompany.Business.Dtos.QuestionDtos;
using JobCompany.Core.Entites;

namespace JobCompany.Business.Dtos.ExamDtos
{
    public record GetExamByIdDto
    {
        public string IntroDescription { get; set; }
        public string LastDescription { get; set; }
        public string Result { get; set; }
        public byte CurrentStep {  get; set; }
        public ICollection<QuestionDetailDto> Questions { get; set; }
    }
}