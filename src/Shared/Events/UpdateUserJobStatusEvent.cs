using SharedLibrary.Enums;

namespace SharedLibrary.Events;

public class UpdateUserJobStatusEvent
{
    public Guid UserId { get; set; }
    public JobStatus JobStatus { get; set; }
}
