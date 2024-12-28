using SharedLibrary.Enums;

namespace SharedLibrary.Responses
{
    public class GetUserSavedVacanciesResponse
    {
        public List<VacancyResponse> Vacancies { get; set; } = new List<VacancyResponse>();
    }

    public class VacancyResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyLocation { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CompanyPhoto { get; set; }
        public decimal? MainSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public int ViewCount { get; set; }
        public bool IsVip { get; set; }
        public bool IsSaved { get; set; }
        public WorkType? WorkType { get; set; }
        // public int TotalCount { get; set; }
    }
}