namespace SharedLibrary.Events;

public class UpdateUserProfileImageEvent
{
    public Guid UserId { get; set; }
    public string? FileName { get; set; }    
    public string? Base64Image { get; set; }
}
