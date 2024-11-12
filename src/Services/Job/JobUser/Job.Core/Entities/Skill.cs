namespace Job.Core.Entities
{
    public class Skill : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<ResumeSkill> ResumeSkills { get; set; }
    }
}