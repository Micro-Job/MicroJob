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
}