using JobCompany.Business.Dtos.NotificationDtos;
using JobCompany.Business.Services.NotificationServices;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Enums;
using SharedLibrary.Events;
using SharedLibrary.Exceptions;
using SharedLibrary.Helpers;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers
{
    public class VacancyAcceptConsumer(JobCompanyDbContext _context, IRequestClient<CheckBalanceRequest> _balanceRequest, IPublishEndpoint _publishEndpoint, INotificationService _notificationService, IConfiguration _configuration) : IConsumer<VacancyAcceptEvent>
    {
        //TODO : burada update olunmun vakansiyani accept ederken problemler var eger vaxti kecmiyibse odemeli deyil ve s kimi seyler nezere alinmayib
        public async Task Consume(ConsumeContext<VacancyAcceptEvent> context)
        {
            var vacancyGuid = Guid.Parse(context.Message.vacancyId);

            var vacancy = await _context.Vacancies.Include(x => x.VacancySkills).Include(x => x.Company).FirstOrDefaultAsync(v => v.Id == vacancyGuid)
                ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));

            var balanceResponse = await _balanceRequest.GetResponse<CheckBalanceResponse>(new CheckBalanceRequest
            {
                InformationType = InformationType.Vacancy,
                UserId = vacancy.Company.UserId
            });

            if ((vacancy.VacancyStatus == VacancyStatus.Pending || 
                vacancy.VacancyStatus == VacancyStatus.Update || 
                vacancy.VacancyStatus == VacancyStatus.PendingActive) && 
                vacancy.EndDate >= DateTime.Now)
            {
                if (balanceResponse.Message.HasEnoughBalance)
                {
                    await _publishEndpoint.Publish(new PayEvent
                    {
                        InformationId = vacancyGuid,
                        InformationType = InformationType.Vacancy,
                        UserId = vacancy.Company.UserId
                    });

                    vacancy.PaymentDate = DateTime.Now.AddDays(1);

                    await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                    {
                        SenderId = null,
                        InformationId = vacancyGuid,
                        NotificationType = NotificationType.VacancyAccept,
                        InformationName = vacancy.Title,
                        ReceiverId = (Guid)vacancy.CompanyId
                    });
                    if (vacancy.VacancyStatus == VacancyStatus.Update)
                    {
                        var appliedUserIds = await _context.Applications
                         .Where(a => !a.IsDeleted && a.VacancyId == vacancyGuid)
                         .Select(a => a.UserId) 
                         .ToListAsync();

                        await _publishEndpoint.Publish(new NotificationToUserEvent
                        {
                            SenderId = vacancy.Company.UserId,
                            SenderName = vacancy.Company.CompanyName,
                            SenderImage = $"{_configuration["JobCompany:BaseUrl"]}{vacancy.Company.CompanyLogo}",
                            InformationId = vacancyGuid,
                            NotificationType = NotificationType.VacancyUpdate,
                            InformationName = vacancy.Title,
                            ReceiverIds = appliedUserIds
                        });
                    }
                    if (vacancy.VacancySkills != null && vacancy.VacancyStatus == VacancyStatus.Pending)
                    {
                        await _publishEndpoint.Publish(
                            new VacancyCreatedEvent
                            {
                                SenderId = vacancy.Company.UserId,
                                SkillIds = vacancy.VacancySkills.Select(x => x.SkillId).ToList(),
                                InformationId = vacancy.Id,
                                InformatioName = vacancy.Title,
                            }
                        );
                    }
                    vacancy.VacancyStatus = VacancyStatus.Active;
                }
                else
                {
                    vacancy.VacancyStatus = VacancyStatus.PendingActive;

                    await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                    {
                        SenderId = null,
                        InformationId = vacancyGuid,
                        NotificationType = NotificationType.VacancyPendingActive,
                        InformationName = vacancy.Title,
                        ReceiverId = (Guid)vacancy.CompanyId
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
