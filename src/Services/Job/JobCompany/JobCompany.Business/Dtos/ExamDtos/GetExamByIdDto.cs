using JobCompany.Business.Dtos.QuestionDtos;
using JobCompany.Core.Entites;

namespace JobCompany.Business.Dtos.ExamDtos
{
    public record GetExamByIdDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string Title { get; set; }
        public bool IsTemplate { get; set; }
        public string IntroDescription { get; set; }
        public string LastDescription { get; set; }
        public string? Result { get; set; }
        public byte CurrentStep { get; set; }
        //public ICollection<QuestionDetailDto> ExamQuestions { get; set; }
        public byte? Duration { get; set; }
    }
}