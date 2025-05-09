namespace SharedLibrary.Responses;

public class GetUserResumePhotoResponse
{
    public Guid ResumeId { get; set; }
    public string? ImageUrl { get; set; }
}
