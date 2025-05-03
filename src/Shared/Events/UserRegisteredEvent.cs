using SharedLibrary.Enums;

namespace SharedLibrary.Events;

public class UserRegisteredEvent
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public JobStatus JobStatus { get; set; }
}