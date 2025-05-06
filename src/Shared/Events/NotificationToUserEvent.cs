using SharedLibrary.Enums;

namespace SharedLibrary.Events;

/// <summary>
/// Userə bildiriş göndərmək üçün istifadə olunan event
/// </summary>
public class NotificationToUserEvent
{
    public List<Guid> ReceiverIds { get; set; } = [];
    public Guid? SenderId { get; set; }
    public Guid InformationId { get; set; }
    public string? InformationName { get; set; }
    public string? SenderName { get; set; }
    public string? SenderImage { get; set; }
    public NotificationType NotificationType { get; set; }
}
