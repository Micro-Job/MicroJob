using JobCompany.Core.Entites;

namespace JobCompany.Business.Dtos.ExamDtos
{
    public record GetExamByIdDto
    {
        public string LogoUrl { get; set; }
        public string IntroDescription { get; set; }
        public string LastDescription { get; set; }
        public string Result { get; set; }
        public ICollection<Question> Questions { get; set; }// dto olacaq
    }
}