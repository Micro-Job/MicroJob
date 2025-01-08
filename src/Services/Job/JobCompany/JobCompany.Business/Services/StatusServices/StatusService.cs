using System.Security.Claims;
using JobCompany.Business.Dtos.StatusDtos;
using JobCompany.Business.Exceptions.StatusExceptions;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace JobCompany.Business.Services.StatusServices
{
    public class StatusService : IStatusService
    {
        private readonly JobCompanyDbContext _context;
        private readonly Guid userGuid;
        private readonly IHttpContextAccessor _contextAccessor;

        public StatusService(JobCompanyDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            userGuid = Guid.Parse(
                _contextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.Sid)?.Value
            );
        }

        public async Task CreateStatusAsync(CreateStatusDto dto)
        {
            var companyId = await _context
                .Companies.Where(x => x.UserId == userGuid)
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
                    .Statuses.Include(s => s.Company)
                    .FirstOrDefaultAsync(x => x.Id == statusGuid && x.Company.UserId == userGuid)
                ?? throw new SharedLibrary.Exceptions.NotFoundException<Status>();

            if (existStatus.IsDefault == true)
                throw new StatusPermissionException();
            var isLinked = await _context.Applications.AnyAsync(a => a.StatusId == statusGuid);
            if (isLinked)
                throw new StatusPermissionException(
                    "Bu status başqa müraciətlərdə istifadə olunur."
                );

            _context.Statuses.Remove(existStatus);
            await _context.SaveChangesAsync();
        }

        public async Task<List<StatusListDto>> GetAllStatusesAsync()
        {
            var statuses = await _context
                .Statuses.Where(s => s.Company.UserId == userGuid)
                .ToListAsync();

            return statuses
                .Select(s => new StatusListDto
                {
                    StatusId = s.Id,
                    StatusName = s.StatusName,
                    StatusColor = s.StatusColor,
                })
                .ToList();
        }
    }
}
