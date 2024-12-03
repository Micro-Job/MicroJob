using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Shared.Events;

namespace JobCompany.Business.Consumers
{
    public class VacancyApplicationConsumer(JobCompanyDbContext _context) : IConsumer<VacancyApplicationEvent>
    {
        public Task Consume(ConsumeContext<VacancyApplicationEvent> context)
        {
            var newNotification = new Notification
            {
                ReceiverId = context.Message.UserId,
                Content = context.Message.Content,
                IsSeen = false
            };
            _context.Notifications.Add(newNotification);
            return _context.SaveChangesAsync();
        }
    }
}