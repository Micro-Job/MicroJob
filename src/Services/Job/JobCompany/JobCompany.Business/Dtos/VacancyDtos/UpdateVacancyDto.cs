using JobCompany.Core.Enums;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Enums;

namespace JobCompany.Business.Dtos.VacancyDtos
{
    public record UpdateVacancyDto
    {
        public string Id {  get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public IFormFile? CompanyLogo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public string? CountryId { get; set; }
        public string? CityId { get; set; }
        public string? Email { get; set; }
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
        //public Guid? VacancyTestId { get; set; }
        public string? CategoryId { get; set; }
    }
}