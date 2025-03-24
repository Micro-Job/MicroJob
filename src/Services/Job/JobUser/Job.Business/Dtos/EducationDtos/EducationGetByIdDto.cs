using Job.Core.Enums;

namespace Job.Business.Dtos.EducationDtos
{
    public record EducationGetByIdDto
    {
        public Guid EducationId { get; set; }
        public string InstitutionName { get; set; }
        public string Profession { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentEducation { get; set; }
        public ProfessionDegree ProfessionDegree { get; set; }
    }
}