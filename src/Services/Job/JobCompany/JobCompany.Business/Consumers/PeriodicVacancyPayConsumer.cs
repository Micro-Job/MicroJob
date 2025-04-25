using JobCompany.Business.Dtos.NotificationDtos;
using JobCompany.Business.Services.NotificationServices;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Enums;
using SharedLibrary.Events;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers
{
    public class PeriodicVacancyPayConsumer
    (
        JobCompanyDbContext _context,
        INotificationService _notificationService,
        IPublishEndpoint _publishEndpoint,
        IRequestClient<CheckBalanceRequest> _balanceRequest
    ) : IConsumer<PeriodicVacancyPayEvent>
    {
        public async Task Consume(ConsumeContext<PeriodicVacancyPayEvent> context)
        {
            var mustPayVacancies = await _context.Vacancies
                .Where(x => x.PaymentDate != null &&
                            x.PaymentDate <= DateTime.Now.AddHours(4) &&
                            x.VacancyStatus == VacancyStatus.Active)
                .Select(x => new
                {
                    x.Id,
                    x.Company.UserId,
                    x.CompanyId,
                    x.Title
                })
                .AsNoTracking()
                .ToListAsync();

            if (mustPayVacancies != null)
            {
                foreach (var item in mustPayVacancies)
                {
                    var response = await _balanceRequest.GetResponse<CheckBalanceResponse>(new CheckBalanceRequest
                    {
                        UserId = item.UserId,
                        InformationType = InformationType.Vacancy
                    });

                    if (response.Message.HasEnoughBalance)
                    {
                        await _publishEndpoint.Publish(new PayEvent
                        {
                            UserId = item.UserId,
                            InformationId = item.Id,
                            InformationType = InformationType.Vacancy,
                        });

                        await _context.Vacancies.Where(v => v.Id == item.Id)
                        .ExecuteUpdateAsync(setter => setter.SetProperty(v => v.PaymentDate, v => v.PaymentDate.Value.AddDays(1)));

                        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                        {
                            InformationId = item.Id,
                            InformationName = item.Title,
                            NotificationType = NotificationType.VacancySuccessDailyPayment,
                            ReceiverId = (Guid)item.CompanyId,
                            SenderId = null
                        });
                    }
                    else
                    {
                        await _context.Vacancies.Where(v => v.Id == item.Id)
                        .ExecuteUpdateAsync(setter => setter
                        .SetProperty(v => v.PaymentDate, (DateTime?)null)
                        .SetProperty(v => v.VacancyStatus, VacancyStatus.PendingActive));

                        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                        {
                            InformationId = item.Id,
                            InformationName = item.Title,
                            NotificationType = NotificationType.VacancyFailedDailyPayment,
                            ReceiverId = (Guid)item.CompanyId,
                            SenderId = null
                        });
                    }
                }
            }

            await context.ConsumeCompleted;
            await _context.SaveChangesAsync();
        }
    }
}