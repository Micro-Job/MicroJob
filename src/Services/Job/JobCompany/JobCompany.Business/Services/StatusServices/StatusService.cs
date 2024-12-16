using JobCompany.Business.Dtos.StatusDtos;
using JobCompany.Business.Exceptions.StatusExceptions;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            userGuid = Guid.Parse(_contextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.Sid)?.Value);
        }

        public async Task CreateStatusAsync(CreateStatusDto dto)
        {
            //deyisilmelidi
            var companyId = _context.Companies.FirstOrDefaultAsync(x => x.UserId == userGuid).Result.Id;

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

            var existStatus = await _context.Statuses.FirstOrDefaultAsync(x => x.Id == statusGuid)
                ?? throw new SharedLibrary.Exceptions.NotFoundException<Status>();

            if (existStatus.IsDefault == true) throw new StatusPermissionException();

            _context.Statuses.Remove(existStatus);
            await _context.SaveChangesAsync();
        }

        public async Task<List<StatusListDto>> GetAllStatusesAsync()
        {
            var company = await _context.Companies.FirstOrDefaultAsync(x => x.UserId == userGuid);

            var allStatus = await _context.Statuses.Where(x => x.IsDefault == true && x.CompanyId == company.Id)
            .Select(x => new StatusListDto
            {
                StatusId = x.Id,
                StatusName = x.StatusName,
                StatusColor = x.StatusColor,
            }).ToListAsync();

            return allStatus;
        }
    }
}