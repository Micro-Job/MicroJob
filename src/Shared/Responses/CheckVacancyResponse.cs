namespace SharedLibrary.Responses;

public class CheckVacancyResponse
{
    public bool IsExist { get; set; }
    public Guid CompanyId { get; set; }
    public string VacancyName { get; set; }
    public bool IsUserApplied { get; set; }
}
