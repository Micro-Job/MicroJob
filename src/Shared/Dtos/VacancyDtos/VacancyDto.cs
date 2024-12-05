using SharedLibrary.Enums;

namespace SharedLibrary.Dtos.VacancyDtos
{
    public class VacancyDto
    {
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public string? CompanyLogo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public int? ViewCount { get; set; }
        public decimal? MainSalary { get; set; }
        public WorkType? WorkType { get; set; }
        public bool IsSaved { get; set; }
        public bool IsVip { get; set; }
    }
}