
namespace Shared.Responses;

public class GetResumesDataResponse
{
    public List<GetResumeDataResponse> Users { get; set; } = new List<GetResumeDataResponse>();
}

public class GetResumeDataResponse
{
    public Guid UserId { get; set; }
    public string? Position { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfileImage { get; set; }
}