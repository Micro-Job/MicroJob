namespace JobCompany.Business.Dtos.ReportDtos
{
    public record ApplicationDetailDto
    {
        public string Position { get; set; }
        public int ApplicationsCount { get; set; }
    }
}