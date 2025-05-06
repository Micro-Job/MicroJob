using SharedLibrary.Enums;

namespace Job.Business.Dtos.NotificationDtos;

public record NotificationListDto
{
    public Guid Id { get; set; }
    public Guid ReceiverId { get; set; }
    public Guid? SenderId { get; set; }
    public string? SenderName { get; set; }
    public string? SenderImage { get; set; }
    public Guid InformationId { get; set; }
    public NotificationType NotificationType { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? InformationName { get; set; }
    public bool IsSeen { get; set; }
}
