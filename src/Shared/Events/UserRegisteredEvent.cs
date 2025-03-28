using SharedLibrary.Enums;

namespace SharedLibrary.Events
{
    public class UserRegisteredEvent
    {
        public Guid UserId { get; set; }
        public JobStatus JobStatus { get; set; }
    }
}