namespace Job.Core.Entities
{
    public class ResumeSkill
    {
        public Guid ResumeId { get; set; }
        public Resume Resume { get; set; }

        public Guid SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}