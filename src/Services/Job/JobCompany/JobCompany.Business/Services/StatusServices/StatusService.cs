using System.Security.Claims;
using JobCompany.Business.Dtos.StatusDtos;
using JobCompany.Business.Exceptions.StatusExceptions;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.StatusServices
{
    public class StatusService(JobCompanyDbContext _context , ICurrentUser _currentUser) : IStatusService
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
                StatusName = dto.StatusName.Trim(),
                StatusColor = dto.StatusColor.Trim(),
                CompanyId = companyId,
                Order = dto.Order,
            };

            await _context.Statuses.AddAsync(newStatus);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStatusAsync(string statusId)
        {
            var statusGuid = Guid.Parse(statusId);

            var existStatus =
                await _context
                    .Statuses.Include(s => s.Applications)
                    .FirstOrDefaultAsync(x => x.Id == statusGuid && x.Company.UserId == _currentUser.UserGuid) ?? throw new SharedLibrary.Exceptions.NotFoundException<Status>();

            if (existStatus.IsDefault == true)
                throw new StatusPermissionException();

            if(existStatus.Applications == null || existStatus.Applications.Count > 0) 
                                throw new StatusPermissionException("Bu status başqa müraciətlərdə istifadə olunur.");

            _context.Statuses.Remove(existStatus);
            await _context.SaveChangesAsync();
        }

        public async Task<List<StatusListDto>> GetAllStatusesAsync()
        {
            var statuses = await _context
                .Statuses.Where(s => s.Company.UserId == _currentUser.UserGuid)
                .Select(s => new StatusListDto
                {
                    StatusId = s.Id,
                    StatusName = s.StatusName,
                    StatusColor = s.StatusColor,
                })
                .ToListAsync();

            return statuses;
        }
    }
}
