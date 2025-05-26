namespace SharedLibrary.Events;

public class UpdateUserInfoEvent
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
