namespace JobCompany.Business.Dtos.ReportDtos
{
    public record ApplicationReportDetailDto
    {
        public string Position { get; set; }
        public int ApplicationsCount { get; set; }
    }
}