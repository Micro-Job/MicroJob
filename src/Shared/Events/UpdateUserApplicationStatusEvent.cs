using SharedLibrary.Enums;

namespace Shared.Events
{
    public class UpdateUserApplicationStatusEvent
    {
        public Guid UserId { get; set; }
        public Guid SenderId { get; set; }
        public Guid InformationId { get; set; }
        public string? InformationName { get; set; }
        public NotificationType NotificationType { get; set; }
    }
}
