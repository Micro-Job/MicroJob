using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites.ExamEntities
{
    public class Template : BaseEntity
    {
        public string Name { get; set; }
        public int ViewCount { get; set; }
        public ICollection<Exam> Exams { get; set; }
    }
}