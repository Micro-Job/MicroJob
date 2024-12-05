using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class Exam : BaseEntity
    {
        public string LogoUrl { get; set; }
        public Template? Template { get; set; }
        public Guid? TemplateId { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}