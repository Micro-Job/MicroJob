using JobCompany.Business.Dtos.SkillDtos;
using Shared.Enums;
using SharedLibrary.Enums;

namespace JobCompany.Business.Dtos.VacancyDtos;

public class VacancyDetailsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string CompanyName { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? ExamId { get; set; }
    public Guid CompanyUserId { get; set; }
    public string? CompanyLogo { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? Location { get; set; }
    public int ViewCount { get; set; }
    public string? Email { get; set; }
    public WorkType? WorkType { get; set; }
    public WorkStyle? WorkStyle { get; set; }
    public decimal? MainSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public string Requirement { get; set; }
    public string? Description { get; set; }
    public Gender Gender { get; set; }
    public Military Military { get; set; }
    public Driver Driver { get; set; }
    public FamilySituation Family { get; set; }
    public Citizenship Citizenship { get; set; }
    public VacancyStatus VacancyStatus { get; set; }
    public bool IsVip { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid? CountryId { get; set; }
    public string? CountryName { get; set; }
    public Guid? CityId { get; set; }
    public string? CityName { get; set; }
    public ICollection<VacancyNumberDto>? VacancyNumbers { get; set; }
    public ICollection<SkillDto> Skills { get; set; }
}
