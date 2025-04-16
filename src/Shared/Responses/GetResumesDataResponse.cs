using Shared.Enums;
using SharedLibrary.Enums;

namespace Shared.Responses;

public class GetResumesDataResponse
{
    public List<GetResumeDataResponse> Users { get; set; } = [];
}

public class GetResumeDataResponse
{
    public Guid UserId { get; set; }
    public string? Position { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfileImage { get; set; }
    public string? Email { get; set; }
    public List<string> PhoneNumber { get; set; }
    public DateTime BirthDay { get; set; }
    public Driver IsDriver { get; set; }
    public FamilySituation IsMarried { get; set; }
    public Military MilitarySituation { get; set; }
}