using JobCompany.Business.Dtos.NotificationDtos;
using JobCompany.Business.Services.NotificationServices;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Enums;
using SharedLibrary.Events;
using SharedLibrary.Exceptions;
using SharedLibrary.Helpers;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Consumers
{
    public class VacancyAcceptConsumer(JobCompanyDbContext _context , IRequestClient<CheckBalanceRequest> _balanceRequest , IPublishEndpoint _publishEndpoint , INotificationService _notificationService) : IConsumer<VacancyAcceptEvent>
    {
        public async Task Consume(ConsumeContext<VacancyAcceptEvent> context)
        {
            var vacancyGuid = Guid.Parse(context.Message.vacancyId);

            var vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == vacancyGuid)
                ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));

            var balanceResponse = await _balanceRequest.GetResponse<CheckBalanceResponse>(new CheckBalanceRequest
            {
                InformationType = InformationType.Vacancy,
                UserId = vacancy.Company.UserId
            });

            if (vacancy.VacancyStatus == VacancyStatus.Pending || vacancy.VacancyStatus == VacancyStatus.Update)
            {
                if (balanceResponse.Message.HasEnoughBalance)
                {
                    await _publishEndpoint.Publish(new PayEvent
                    {
                        InformationId = vacancyGuid,
                        InformationType = InformationType.Vacancy,
                        UserId = vacancy.Company.UserId
                    });
                    await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                    {
                        SenderId = null,
                        InformationId = vacancyGuid,
                        NotificationType = NotificationType.VacancyAccept,
                        InformationName = vacancy.Title,
                        ReceiverId = vacancy.Company.UserId
                    });
                    if (vacancy.VacancyStatus == VacancyStatus.Update)
                    {
                        var appliedUserIds = await _context.Applications
                         .Where(a => !a.IsDeleted && a.VacancyId == vacancyGuid)
                         .Select(a => a.UserId)
                         .ToListAsync();

                        //TODO : burada notificationlarda ümumi olaraq problem var.Bu formada olmalı deyil(Yeni ki hem userin hem
                        //de sirketin notificationları olmalı deyil bir yerden idare edilmelidir.Buradaki kimi problemlere getirib cixarir)
                        await _notificationService.CreateBulkNotificationAsync(new CreateBulkNotificationDto
                        {
                            SenderId = vacancy.Company.UserId,
                            InformationId = vacancyGuid,
                            NotificationType = NotificationType.VacancyAccept,
                            InformationName = vacancy.Title,
                            ReceiverIds = appliedUserIds
                        });
                    }

                    vacancy.PaymentDate = DateTime.Now.AddDays(1);
                    vacancy.VacancyStatus = VacancyStatus.Active;
                }
                else
                {
                    await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                    {
                        SenderId = null,
                        InformationId = vacancyGuid,
                        NotificationType = NotificationType.VacancyPendingActive,
                        InformationName = vacancy.Title,
                        ReceiverId = vacancy.Company.UserId
                    });
                    vacancy.VacancyStatus = VacancyStatus.PendingActive;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
