namespace JobCompany.Business.Dtos.ReportDtos
{
    public record ApplicationStatisticsDto
    {
        public int TotalApplications { get; set; }
        public PercentageChangeDto PercentageChange { get; set; }
        public List<PeriodStatisticDto> PeriodStatistics { get; set; }
        public List<ApplicationReportDetailDto> Applications { get; set; }
    }
}