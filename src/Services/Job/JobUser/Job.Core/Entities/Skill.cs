namespace Job.Core.Entities
{
    public class Skill : BaseEntity
    {
        public ICollection<ResumeSkill> ResumeSkills { get; set; }
        public ICollection<SkillTranslation> Translations { get; set; }
    }
}