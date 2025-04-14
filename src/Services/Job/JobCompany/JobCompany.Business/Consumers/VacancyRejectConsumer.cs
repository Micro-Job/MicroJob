using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using SharedLibrary.Events;

namespace JobCompany.Business.Consumers;

public class VacancyRejectConsumer(JobCompanyDbContext _context) : IConsumer<VacancyRejectedEvent>
{
    public async Task Consume(ConsumeContext<VacancyRejectedEvent> context)
    {
        var newNotification = new Notification
        {
            ReceiverId = context.Message.ReceiverId ?? throw new Exception(),
            SenderId = context.Message.SenderId,
            NotificationType = SharedLibrary.Enums.NotificationType.VacancyReject,
            InformationId = context.Message.InformationId,
            InformationName = context.Message.InformationName,
            IsSeen = false,
        };
        await _context.Notifications.AddAsync(newNotification);
        await _context.SaveChangesAsync();
    }
}
