using JobCompany.Core.Entites.Base;
using JobCompany.Core.Enums;
using SharedLibrary.Enums;

namespace JobCompany.Core.Entites
{
    public class Vacancy : BaseEntity
    {
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public string? CompanyLogo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public int? ViewCount { get; set; }
        public string? Email { get; set; }
        public WorkType? WorkType { get; set; }
        public decimal? MainSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string Requirement { get; set; }
        public string? Description { get; set; }
        public SharedLibrary.Enums.Gender Gender { get; set; }
        public Military Military { get; set; }
        public Driver Driver { get; set; }
        public SharedLibrary.Enums.FamilySituation Family { get; set; }
        public Citizenship Citizenship { get; set; }
        public bool IsActive { get; set; }
        public bool IsVip { get; set; }

        public Guid? CompanyId { get; set; }
        public Company? Company { get; set; }

        public Guid? CountryId { get; set; }
        public Country? Country { get; set; }

        public Guid? CityId { get; set; }
        public City? City { get; set; }

        public Guid? ExamId { get; set; }
        public Exam? Exam { get; set; }

        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<CompanyNumber>? CompanyNumbers { get; set; }
        public ICollection<Application>? Applications { get; set; }
        public ICollection<VacancySkill> Skills { get; set; }
    }
}