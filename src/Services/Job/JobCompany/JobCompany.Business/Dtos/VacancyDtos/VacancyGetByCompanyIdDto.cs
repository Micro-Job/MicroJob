using SharedLibrary.Enums;

namespace JobCompany.Business.Dtos.VacancyDtos
{
    public record VacancyGetByCompanyIdDto
    {
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public string? Location { get; set; }
        public WorkType? WorkType { get; set; }
        public DateTime StartDate { get; set; }
        public int? ViewCount { get; set; }
        public decimal? MainSalary { get; set; }
        public decimal? MaxSalary { get; set; }
    }
}