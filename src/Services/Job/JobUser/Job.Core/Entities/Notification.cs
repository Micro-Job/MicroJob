using SharedLibrary.Enums;

namespace Job.Core.Entities;

public class Notification : BaseEntity
{
    public Guid ReceiverId { get; set; }
    public User Receiver { get; set; }
    public Guid SenderId { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsSeen { get; set; }
    public Guid InformationId { get; set; }
    public string? InformationName { get; set; }
    public NotificationType NotificationType { get; set; }
}
