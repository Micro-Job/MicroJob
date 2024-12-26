namespace SharedLibrary.Requests;

public class GetOtherVacanciesByCompanyRequest
{
    public Guid CompanyId { get; set; }
    public Guid? CurrentVacancyId { get; set; }
    public int Skip { get; set; } 
    public int Take { get; set; } 
}