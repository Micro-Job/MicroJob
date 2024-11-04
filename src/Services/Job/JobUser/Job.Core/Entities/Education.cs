using Job.Core.Enums;

namespace Job.Core.Entities
{
    public class Education : BaseEntity
    {
        public Resume Resume { get; set; }
        public Guid ResumeId { get; set; }
        public string InstitutionName { get; set; }
        public string Profession { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentEducation { get; set; }
        public ProfessionDegree ProfessionDegree { get; set; }
    }
}