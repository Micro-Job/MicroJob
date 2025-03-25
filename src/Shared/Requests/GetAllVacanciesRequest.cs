using SharedLibrary.Enums;

namespace Shared.Requests
{
    public class GetAllVacanciesRequest
    {
        public string? TitleName { get; set; }
        public string? CategoryId { get; set; }
        public string? CountryId { get; set; }
        public string? CityId { get; set; }
        public VacancyStatus? IsActive { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string? CompanyId { get; set; }
        public WorkType? WorkType { get; set; }
        public WorkStyle? WorkStyle { get; set; }
    }
} 