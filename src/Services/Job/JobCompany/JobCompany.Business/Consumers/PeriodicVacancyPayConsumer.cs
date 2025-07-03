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
        NotificationService _notificationService,
        IPublishEndpoint _publishEndpoint,
        IRequestClient<CheckBalanceRequest> _balanceRequest
    ) : IConsumer<PeriodicVacancyPayEvent>
    {
        public async Task Consume(ConsumeContext<PeriodicVacancyPayEvent> context)
        {
            var dateTimeNow = DateTime.Now.AddHours(4);

            var mustPayVacancies = await _context.Vacancies
                .Where(x => x.PaymentDate != null &&
                            x.PaymentDate <= dateTimeNow &&
                            x.VacancyStatus == VacancyStatus.Active)
                .Select(x => new
                {
                    x.Id,
                    x.Company!.UserId,
                    x.CompanyId,
                    x.EndDate,
                    x.Title
                })
                .AsNoTracking()
                .ToListAsync();

            //TODO : bu hisse optimize olunmalıdır(executeUpdateler problemdir)Procedure istifade edile biler butun datalar yigildiqdan sonra sql terefde
            if (mustPayVacancies != null)
            {
                foreach (var vacancy in mustPayVacancies)
                {
                    //TODO : bu olsun mu ps Nermin:olmasin
                    if(vacancy.EndDate < dateTimeNow)
                    {
                        await _context.Vacancies.Where(v => v.Id == vacancy.Id)
                        .ExecuteUpdateAsync(setter => setter
                        .SetProperty(v => v.PaymentDate, (DateTime?)null)
                        .SetProperty(v => v.VacancyStatus, VacancyStatus.Deactive));
                        continue;
                    }

                    var response = await _balanceRequest.GetResponse<CheckBalanceResponse>(new CheckBalanceRequest
                    {
                        UserId = vacancy.UserId,
                        InformationType = InformationType.Vacancy
                    });

                    if (response.Message.HasEnoughBalance)
                    {
                        await _publishEndpoint.Publish(new PayEvent
                        {
                            UserId = vacancy.UserId,
                            InformationId = vacancy.Id,
                            InformationType = InformationType.Vacancy,
                        });

                        await _context.Vacancies.Where(v => v.Id == vacancy.Id)
                        .ExecuteUpdateAsync(setter => setter.SetProperty(v => v.PaymentDate, v => v.PaymentDate.Value.AddDays(1)));

                        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                        {
                            InformationId = vacancy.Id,
                            InformationName = vacancy.Title,
                            NotificationType = NotificationType.VacancySuccessDailyPayment,
                            ReceiverId = vacancy.CompanyId,
                            SenderId = null
                        });
                    }
                    else
                    {
                        await _publishEndpoint.Publish(new PayEvent
                        {
                            UserId = vacancy.UserId,
                            InformationId = vacancy.Id,
                            InformationType = InformationType.Vacancy,
                        });

                        await _context.Vacancies.Where(v => v.Id == vacancy.Id)
                        .ExecuteUpdateAsync(setter => setter
                        .SetProperty(v => v.PaymentDate, (DateTime?)null)
                        .SetProperty(v => v.VacancyStatus, VacancyStatus.PendingActive));

                        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                        {
                            InformationId = vacancy.Id,
                            InformationName = vacancy.Title,
                            NotificationType = NotificationType.VacancyFailedDailyPayment,
                            ReceiverId = (Guid)vacancy.CompanyId,
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