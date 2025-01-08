using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class Exam : BaseEntity
    {
        public string Title { get; set; }
        public bool IsTemplate { get; set; }
        public string IntroDescription { get; set; }
        public string LastDescription { get; set; }
        public string? Result { get; set; }
        public decimal LimitRate { get; set; }
        public ICollection<Vacancy> Vacancies { get; set; }
        public Company Company { get; set; }
        public Guid CompanyId { get; set; }
        public ICollection<ExamQuestion> ExamQuestions { get; set; }
        public byte? Duration { get; set; }
    }
}
