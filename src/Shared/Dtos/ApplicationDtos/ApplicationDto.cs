namespace SharedLibrary.Dtos.ApplicationDtos;

public class ApplicationDto
{
    public Guid VacancyId { get; set; }
    public string title { get; set; }
    public Guid? CompanyId { get; set; }
    public string companyLogo { get; set; }
    public string CompanyName { get; set; }
    public string? WorkType { get; set; }
    public bool IsActive { get; set; }
    public string? StatusName { get; set; }
    public string? StatusColor { get; set; }
    public int? ViewCount { get; set; }
    public DateTime startDate { get; set; }
}