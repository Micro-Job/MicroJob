namespace SharedLibrary.Responses;

public class GetCompaniesDataByUserIdsResponse
{
    public Dictionary<Guid, CompanyNameAndImageDto> Companies { get; set; } = [];
}

public class CompanyNameAndImageDto
{
    public string? CompanyName { get; set; }
    public string? CompanyLogo { get; set; }
}
