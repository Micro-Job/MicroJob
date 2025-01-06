using SharedLibrary.Dtos.ApplicationDtos;

namespace SharedLibrary.Responses;

public class GetUserApplicationsResponse
{
    public List<ApplicationDto> UserApplications { get; set; }
}
