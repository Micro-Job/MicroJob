using SharedLibrary.Enums;

namespace Shared.Events
{
    public class VacancyUpdatedEvent
    {
        public Guid SenderId { get; set; }
        public Guid InformationId { get; set; }
        public string? InformationName { get; set; }
        public List<Guid> UserIds { get; set; }
        public NotificationType NotificationType { get; set; }
    }
}