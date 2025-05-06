using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using SharedLibrary.Events;

namespace Job.Business.Consumers;

public class NotificationEventConsumer(JobDbContext _jobDbContext) : IConsumer<NotificationToUserEvent>
{
    public async Task Consume(ConsumeContext<NotificationToUserEvent> context)
    {
        var notifications = context
            .Message.ReceiverIds.Select(receiverId => new Notification
            {
                ReceiverId = receiverId,
                SenderId = context.Message.SenderId,
                NotificationType = context.Message.NotificationType,
                InformationId = context.Message.InformationId,
                InformationName = context.Message.InformationName,
                SenderImage = context.Message.SenderImage,
                SenderName = context.Message.SenderName,
                CreatedDate = DateTime.UtcNow,
                IsSeen = false,
            }).ToList();

        await _jobDbContext.Notifications.AddRangeAsync(notifications);
        await _jobDbContext.SaveChangesAsync();
    }
}
