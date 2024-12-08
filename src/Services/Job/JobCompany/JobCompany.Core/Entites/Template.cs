using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class Template : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<Exam> Exams { get; set; }
    }
}