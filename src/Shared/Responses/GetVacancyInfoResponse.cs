using Shared.Dtos.CompanyDtos;
using Shared.Enums;
using SharedLibrary.Dtos.CategoryDtos;
using SharedLibrary.Enums;

namespace SharedLibrary.Responses
{
    public class GetVacancyInfoResponse
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public string CompanyLogo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public string? Email { get; set; }
        public string? CategoryName { get; set; }
        public WorkType? WorkType { get; set; }
        public int? ViewCount { get; set; }
        public decimal? MainSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string Requirement { get; set; }
        public string? Description { get; set; }
        public List<string> VacancyNumbers { get; set; }
        public Gender Gender { get; set; }
        public Military Military { get; set; }
        public Driver Driver { get; set; }
        public FamilySituation Family { get; set; }
        public Citizenship Citizenship { get; set; }
        public bool IsActive { get; set; }
    }
}