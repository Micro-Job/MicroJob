using JobCompany.Core.Enums;
using SharedLibrary.Enums;

namespace JobCompany.Business.Dtos.VacancyDtos
{
    public record VacancyGetAllDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? CompanyLogo { get; set; }
        public string? CompanyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        //public string? Location { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public int? ViewCount { get; set; }
        public WorkType? WorkType { get; set; }
        public WorkStyle? WorkStyle { get; set; }
        public decimal? MainSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public SalaryCurrencyType? SalaryCurrency { get; set; }
        public bool IsSaved { get; set; }
        public VacancyStatus VacancyStatus { get; set; }
    }
}
