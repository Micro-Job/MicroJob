namespace SharedLibrary.Requests;

public class GetCompaniesDataByUserIdsRequest
{
    public List<Guid> UserIds { get; set; } = [];
}
