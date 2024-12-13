using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class VacancySkill : BaseEntity
    {
        public Guid VacancyId { get; set; }
        public Vacancy Vacancy { get; set; }

        public Guid SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}