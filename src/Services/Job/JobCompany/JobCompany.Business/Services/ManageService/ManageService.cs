using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.MessageDtos;
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.Business.Exceptions.Common;
using JobCompany.Business.Extensions;
using JobCompany.Business.Statistics;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Enums;
using SharedLibrary.Events;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.ManageService;

public class ManageService(JobCompanyDbContext _context, ICurrentUser _currentUser, IPublishEndpoint _publishEndpoint) : IManageService
{
    public async Task VacancyAcceptAsync(string vacancyId)
    {
        await _publishEndpoint.Publish(new VacancyAcceptEvent
        {
            vacancyId = vacancyId
        });
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
                SenderId = _currentUser.UserGuid,
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
                SenderId = (Guid)_currentUser.UserGuid,
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


    public async Task<DataListDto<MessageWithTranslationsDto>> GetAllMessagesAsync(int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Messages.AsQueryable();

        var totalCount = await query.CountAsync();

        var messageDtos = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MessageWithTranslationsDto
            {
                Id = m.Id,
                CreatedDate = m.CreatedDate,
                Translations = m.Translations.Select(t => new MessageTranslationDto
                {
                    Language = t.Language,
                    Content = t.Content
                }).ToList()
            })
            .ToListAsync();

        return new DataListDto<MessageWithTranslationsDto>
        {
            Datas = messageDtos,
            TotalCount = totalCount
        };
    }

    public async Task<MessageDto> GetMessageByIdAsync(string id)
    {
        var messageGuid = Guid.Parse(id);
        var message = await _context.Messages
            .Include(m => m.Translations)
            .FirstOrDefaultAsync(m => m.Id == messageGuid)
            ?? throw new NotFoundException<Message>(MessageHelper.GetMessage("NOT_FOUND"));

        return new MessageDto
        {
            Id = message.Id,
            CreatedDate = message.CreatedDate,
            Content = message.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Content)
        };
    }

    public async Task CreateMessageAsync(CreateMessageDto dto)
    {
        var message = new Message
        {
            CreatedDate = DateTime.Now,
            Translations = dto.Translations.Select(t => new MessageTranslation
            {
                Content = t.Content,
                Language = t.Language
            }).ToList()
        };

        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateMessageAsync(string id, UpdateMessageDto dto)
    {
        var messageGuid = Guid.Parse(id);
        var message = await _context.Messages
            .Include(m => m.Translations)
            .FirstOrDefaultAsync(m => m.Id == messageGuid)
            ?? throw new NotFoundException<Message>(MessageHelper.GetMessage("NOT_FOUND"));

        foreach (var dtoTranslation in dto.Translations)
        {
            var existingTranslation = message.Translations
                .FirstOrDefault(t => t.Language == dtoTranslation.Language);

            if (existingTranslation != null) // Mövcud olan tərcüməni yeniləyirik
            {
                existingTranslation.Content = dtoTranslation.Content;
            }
            else // Yeni tərcümə əlavə edirik
            {
                message.Translations.Add(new MessageTranslation
                {
                    MessageId = messageGuid,
                    Language = dtoTranslation.Language,
                    Content = dtoTranslation.Content
                });
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteMessageAsync(string id)
    {
        var messageGuid = Guid.Parse(id);
        var message = await _context.Messages
            .Include(m => m.Translations)
            .FirstOrDefaultAsync(m => m.Id == messageGuid)
            ?? throw new NotFoundException<Message>(MessageHelper.GetMessage("NOT_FOUND"));

        _context.Messages.Remove(message);
        await _context.SaveChangesAsync();
    }
}
