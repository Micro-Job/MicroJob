namespace SharedLibrary.Responses;

public class CheckVacancyResponse
{
    public bool IsExist { get; set; }
    public Guid CompanyId { get; set; }
}