namespace Job.Business.Dtos.ExperienceDtos
{
    public class ExperienceGetByIdDto
    {
        public string OrganizationName { get; set; }
        public string PositionName { get; set; }
        public string? PositionDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentOrganization { get; set; }
    }
}