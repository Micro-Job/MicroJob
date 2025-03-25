using SharedLibrary.Enums;

namespace SharedLibrary.Dtos.ApplicationDtos;

public class ApplicationDto
{
    public Guid ApplicationId { get; set; }
    public Guid VacancyId { get; set; }
    public string Title { get; set; }
    public Guid? CompanyId { get; set; }
    public string CompanyLogo { get; set; }
    public string CompanyName { get; set; }
    public string? WorkType { get; set; }
    public VacancyStatus VacancyStatus { get; set; }
    public string? StatusName { get; set; }
    public string? StatusColor { get; set; }
    public int? ViewCount { get; set; }
    public DateTime StartDate { get; set; }
    public decimal? MainSalary { get; set; }
    public decimal? MaxSalary { get; set; }
}
