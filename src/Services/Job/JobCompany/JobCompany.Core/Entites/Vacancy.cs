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
        public Guid CountryId { get; set; }
        public Country Country { get; set; }
        public Guid CityId { get; set; }
        public City City { get; set; }
        public string? Email { get; set; }
        public ICollection<CompanyNumber>? CompanyNumbers { get; set; }
        public WorkType? WorkType { get; set; }
        public decimal? MainSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string Requirement { get; set; }
        public string Description { get; set; }
        public Gender Gender { get; set; }
        public Military Military { get; set; }
        public Driver Driver { get; set; }
        public FamilySituation Family { get; set; }
        public Citizenship Citizenship { get; set; }
        public bool IsActive { get; set; }
        public Guid? VacancyTestId { get; set; }
        public VacancyTest? VacancyTest { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
    }
}