namespace Job.Core.Entities
{
    public class Experience : BaseEntity
    {
        public Resume Resume { get; set; }
        public Guid ResumeId { get; set; }
        public string OrganizationName { get; set; }
        public string PositionName { get; set; }
        public string? PositionDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentOrganization { get; set; }

    }
}