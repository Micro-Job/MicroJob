using System.Security.Claims;
using Job.Core.Entities;
using JobCompany.Business.Dtos.StatusDtos;
using JobCompany.Business.Exceptions.StatusExceptions;
using JobCompany.Business.Extensions;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.StatusServices
{
    public class StatusService(JobCompanyDbContext _context, ICurrentUser _currentUser) : IStatusService
    {
        public async Task CreateStatusAsync(CreateStatusDto dto)
        {
            var companyId = await _context
                .Companies.Where(x => x.UserId == _currentUser.UserGuid)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (companyId == Guid.Empty)
                throw new SharedLibrary.Exceptions.NotFoundException<Company>();

            var newStatus = new Status
            {
                IsDefault = false,
                StatusColor = dto.StatusColor.Trim(),
                CompanyId = companyId,
                Order = dto.Order,
            };

            await _context.Statuses.AddAsync(newStatus);
            await _context.SaveChangesAsync();

            var statusTranslations = dto.Statuses.Select(x => new StatusTranslation
            {
                StatusId = newStatus.Id,
                Language = x.Language,
                Name = x.Name.Trim()
            }).ToList();

            await _context.StatusTranslations.AddRangeAsync(statusTranslations);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStatusAsync(string statusId)
        {
            var statusGuid = Guid.Parse(statusId);

            var existStatus =
                await _context
                    .Statuses.Include(s => s.Applications).Include(x => x.Translations)
                    .FirstOrDefaultAsync(x => x.Id == statusGuid && x.Company.UserId == _currentUser.UserGuid) ?? throw new SharedLibrary.Exceptions.NotFoundException<Status>();

            if (existStatus.IsDefault == true)
                throw new StatusPermissionException();

            if (existStatus.Applications == null || existStatus.Applications.Count > 0)
                throw new StatusPermissionException(MessageHelper.GetMessage("STATUS_PERMISSION"));


            var statusTranslations = existStatus.Translations.Select(x => x).ToList();
            _context.StatusTranslations.RemoveRange(statusTranslations);
            _context.Statuses.Remove(existStatus);
            await _context.SaveChangesAsync();
        }

        public async Task<List<StatusListDto>> GetAllStatusesAsync()
        {
            var statuses = await _context.Statuses
            .IncludeTranslations()
            .Select(b => new StatusListDto
            {
                StatusId = b.Id,
                StatusName = b.GetTranslation(_currentUser.LanguageCode),
                StatusColor = b.StatusColor,
                Order = b.Order,
            })
            .ToListAsync();

            return statuses;
        }

        public async Task UpdateStatusAsync(List<UpdateStatusDto> dtos)
        {
            var statusIds = dtos.Select(dto => dto.Id).ToList();

            var existingStatuses = await _context.Statuses
                .Where(s => statusIds.Contains(s.Id))
                .Include(s => s.Translations)
                .ToListAsync();

            if (!existingStatuses.Any())
                throw new SharedLibrary.Exceptions.NotFoundException<Status>();

            existingStatuses.ForEach(existingStatus =>
            {
                var dto = dtos.FirstOrDefault(d => d.Id == existingStatus.Id);
                if (dto == null) return;

                existingStatus.StatusColor = dto.StatusColor.Trim();
                existingStatus.Order = dto.Order;

                var existingTranslations = existingStatus.Translations.ToDictionary(t => t.Language, t => t);

                var newTranslations = new List<StatusTranslation>();

                foreach (var newTranslation in dto.Statuses)
                {
                    if (existingTranslations.TryGetValue(newTranslation.Language, out var existingTranslation))
                    {
                        existingTranslation.Name = newTranslation.Name.Trim();
                    }
                    else
                    {
                        newTranslations.Add(new StatusTranslation
                        {
                            StatusId = existingStatus.Id,
                            Language = newTranslation.Language,
                            Name = newTranslation.Name.Trim()
                        });
                    }
                }

                _context.StatusTranslations.AddRange(newTranslations);


                _context.StatusTranslations.AddRange(newTranslations);
            });

            await _context.SaveChangesAsync();
        }
    }
}
