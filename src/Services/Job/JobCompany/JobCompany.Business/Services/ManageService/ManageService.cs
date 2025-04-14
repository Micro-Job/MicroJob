
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.Business.Exceptions.Common;
using JobCompany.Business.Exceptions.VacancyExceptions;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using SharedLibrary.Enums;
using SharedLibrary.Events;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.ManageService;

public class ManageService(JobCompanyDbContext _context,ICurrentUser _user, IPublishEndpoint _publishEndpoint) : IManageService
{
    public async Task VacancyAcceptAsync(string vacancyId)
    {
        var vacancyGuid = Guid.Parse(vacancyId);

        var vacancy = await _context.Vacancies
            .Include(v => v.Applications)
            .Where(v => v.Id == vacancyGuid).FirstOrDefaultAsync()
            ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));

        var appliedUserIds = vacancy.Applications
             .Where(a => !a.IsDeleted)
             .Select(a => a.UserId)
             .ToList();

        vacancy.VacancyStatus = VacancyStatus.Active;
        await _context.SaveChangesAsync();

        ///summary
        /// Vakansiya accept olunanda sirkete bildiris getmesi
        ///summary
        await _publishEndpoint.Publish(
            new VacancyAcceptedEvent
            {
                InformationId = vacancyGuid,
                SenderId = null,
                ReceiverId = vacancy.Company.UserId,
                InformationName = vacancy.Title
            }
        );

        ///summary
        /// Vakansiya update olunanda bu vakansiyaya muraciet edenlere bildiris getmesi
        ///summary
        //await _publishEndpoint.Publish(
        //    new VacancyUpdatedEvent
        //    {
        //        InformationId = vacancyGuid,
        //        SenderId = (Guid)_user.UserGuid,
        //        UserIds = appliedUserIds,
        //        InformationName = vacancy.Title,
        //    }
        //);
    }

    public async Task VacancyRejectAsync(VacancyStatusUpdateDto dto)
    {
        var vacancyGuid = Guid.Parse(dto.VacancyId);
        var vacancyCommenGuid = Guid.Parse(dto.VacancyCommentId);
        var vacancy = await _context.Vacancies
            .Include(v => v.Applications)
            .Where(v => v.Id == vacancyGuid).FirstOrDefaultAsync()
            ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));

        var appliedUserIds = vacancy.Applications
             .Where(a => !a.IsDeleted)
             .Select(a => a.UserId)
             .ToList();

        ///summary
        /// Vakansiya reject olunanda bu vakansiyaya olunan muracietlerin isdeleted olunmasi
        ///summary
        await _context.Applications
        .Where(a => a.VacancyId == vacancyGuid)
        .ExecuteUpdateAsync(setters => setters
            .SetProperty(a => a.IsDeleted, true)
        );

        vacancy.VacancyCommentId = vacancyCommenGuid;
        vacancy.VacancyStatus = VacancyStatus.Reject;
        await _context.SaveChangesAsync();

       ///summary
       /// Vakansiya reject olunanda sirket sahibine bildiris getmesi
       ///summary
        await _publishEndpoint.Publish(
            new VacancyRejectedEvent
            {
                InformationId = vacancyGuid,
                SenderId = _user.UserGuid,
                ReceiverId = (Guid)vacancy.CompanyId,
                InformationName = vacancy.Title
            }
        );

        ///summary
        /// Vakansiya reject olunanda bu vakansiyaya muraciet edenlere bildiris getmesi
        ///summary
        await _publishEndpoint.Publish(
            new VacancyDeletedEvent
            {
                InformationId = vacancyGuid,
                SenderId = (Guid)_user.UserGuid,
                UserIds = appliedUserIds,
                InformationName = vacancy.Title,
            }
        );
    }

    public async Task ToggleBlockVacancyStatusAsync(VacancyStatusUpdateDto dto)
    {
        var vacancyGuid = Guid.Parse(dto.VacancyId);
        var vacancy = await _context.Vacancies
            .Where(v => v.Id == vacancyGuid)
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));

        if (vacancy.VacancyStatus != SharedLibrary.Enums.VacancyStatus.Block)
        {
            vacancy.VacancyStatus = SharedLibrary.Enums.VacancyStatus.Block;
            vacancy.VacancyCommentId = Guid.Parse(dto.VacancyCommentId);
        }
        else
        {
            vacancy.VacancyStatus = SharedLibrary.Enums.VacancyStatus.Deactive;

            if (vacancy.VacancyCommentId != null)
            {
                vacancy.VacancyCommentId = null;
            }
        }

        await _context.SaveChangesAsync();
    }
}
