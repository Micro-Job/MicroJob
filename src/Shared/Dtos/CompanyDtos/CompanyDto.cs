namespace SharedLibrary.Dtos.CompanyDtos
{
    public class CompanyDto
    {
        public Guid CompanyId { get; set; }
        public Guid CompanyUserId { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyImage { get; set; }
        public int CompanyVacancyCount { get; set; }
    }

    public class PaginatedCompanyDto
    {
        public ICollection<CompanyDto> Companies { get; set; }
        public int TotalCount { get; set; }
    }
}