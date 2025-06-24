using SharedLibrary.Enums;

namespace SharedLibrary.Events;

public class UserRegisteredEvent
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string MainPhoneNumber { get; set; }
}