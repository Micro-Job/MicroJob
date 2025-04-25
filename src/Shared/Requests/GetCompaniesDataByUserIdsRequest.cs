namespace SharedLibrary.Requests;

public class GetCompaniesDataByUserIdsRequest
{
    public List<Guid> UserIds { get; set; } = [];

    /// <summary>
    /// Şirkət adını filtrləmək üçün optional-dır. Ancaq UserIds siyahısı boş olmadıqda tətbiq edilir.
    /// </summary>
    public string? CompanyName { get; set; }
}
