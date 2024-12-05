namespace JobCompany.Business.Dtos.ReportDtos
{
    public record RecentApplicationDto
    {
        public string Fullname { get; set; }
        public string VacancyName { get; set; }
        public string StatusName { get; set; }
        public string StatusColor { get; set; }
    }
}