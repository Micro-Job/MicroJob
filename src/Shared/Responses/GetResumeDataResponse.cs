using SharedLibrary.Enums;

namespace SharedLibrary.Responses;

public class GetResumeDataResponse
{
    public Guid ResumeId { get; set; }
    public string? Position { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string ProfileImage { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public Gender Gender { get; set; }
}