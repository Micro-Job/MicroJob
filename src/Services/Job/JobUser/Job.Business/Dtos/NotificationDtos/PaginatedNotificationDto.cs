namespace Job.Business.Dtos.NotificationDtos;

public record PaginatedNotificationDto
{
    public List<NotificationListDto> Notifications { get; set; }
    public int TotalCount { get; set; }
}
