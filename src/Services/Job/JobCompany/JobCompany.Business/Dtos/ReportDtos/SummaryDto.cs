namespace JobCompany.Business.Dtos.ReportDtos
{
    public record SummaryDto
    {
        public int ActiveVacancies { get; set; }
        public int TotalApplications { get; set; }
        public int AcceptedApplications { get; set; }
    }
}