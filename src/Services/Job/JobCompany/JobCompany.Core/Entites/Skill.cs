using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class Skill : BaseEntity
    {
        public ICollection<VacancySkill> VacancySkills { get; set; }

        public ICollection<SkillTranslation> Translations { get; set; }

    }
}