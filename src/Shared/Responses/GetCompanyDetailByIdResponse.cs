using SharedLibrary.Dtos.CompanyDtos;

namespace SharedLibrary.Responses;

public class GetCompanyDetailByIdResponse
{
    public string? CompanyInformation { get; set; }
    public string CompanyName { get; set; }
    public string? CompanyLogo { get; set; }
    public IEnumerable<CompanyNumberDto>? CompanyNumbers { get; set; }
    public string? CompanyLocation { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string? WebLink { get; set; }
}