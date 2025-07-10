namespace Job.Core.Entities
{
    public class Skill : BaseEntity
    {
        public bool IsSoft { get; set; }

        public ICollection<ResumeSkill> ResumeSkills { get; set; }
        public ICollection<SkillTranslation> Translations { get; set; }
    }
}