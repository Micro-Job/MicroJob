using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class Skill : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<VacancySkill> VacancySkills { get; set; }
    }
}