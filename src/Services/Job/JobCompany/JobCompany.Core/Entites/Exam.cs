using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class Exam : BaseEntity
    {
        public string LogoUrl { get; set; }
        public Template? Template { get; set; }
        public Guid? TemplateId { get; set; }
        public string IntroDescription { get; set; }//girish melumat metni
        public string LastDescription { get; set; }//sondaki melumatlandirma metni
        public string Result { get; set; }//imtahan neticesi metni
        public ICollection<Question> Questions { get; set; }
        public ICollection<Vacancy> Vacancies { get; set; }
    }
}