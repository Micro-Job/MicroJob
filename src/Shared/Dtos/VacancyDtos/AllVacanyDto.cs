using SharedLibrary.Enums;

namespace Shared.Dtos.VacancyDtos
{
    public class AllVacanyDto
    {
        public string VacancyId { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public string? CompanyLogo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public int? ViewCount { get; set; }
        public decimal? MainSalary { get; set; }
        public WorkType? WorkType { get; set; }
        public WorkStyle? WorkStyle { get; set; }
        public bool IsSaved { get; set; }
        public bool IsVip { get; set; }
        public bool IsActive { get; set; }
        public Guid? CategoryId { get; set; }
        // public CompanyDetailDto Company { get; set; }
    }

    public class PaginatedVacancyDto
    {
        public ICollection<AllVacanyDto> Vacancies { get; set; }
        public int TotalCount { get; set; }
    }
}