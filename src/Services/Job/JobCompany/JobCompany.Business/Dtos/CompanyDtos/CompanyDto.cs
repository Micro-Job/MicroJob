namespace JobCompany.Business.Dtos.CompanyDtos
{
    public record CompanyDto
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyImage { get; set; }
        public int CompanyVacancyCount { get; set; }
    }
}