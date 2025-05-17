namespace SharedLibrary.Responses;

public class GetResumeDataResponse
{
    public Guid ResumeId { get; set; }
    public string? Position { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfileImage { get; set; }
    public string? Email { get; set; }
    public List<string> PhoneNumber { get; set; }
}