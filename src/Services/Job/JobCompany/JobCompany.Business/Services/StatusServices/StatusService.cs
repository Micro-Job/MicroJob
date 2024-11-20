using AuthService.Business.Exceptions.UserException;
using AuthService.Business.Services.CurrentUser;
using JobCompany.Business.Dtos.StatusDtos;
using JobCompany.Business.Exceptions.StatusExceptions;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace JobCompany.Business.Services.StatusServices
{
    public class StatusService : IStatusService
    {
        private readonly JobCompanyDbContext _context;
        private readonly ICurrentUser _currentUser;
        private readonly Guid userGuid;

        public StatusService(JobCompanyDbContext context, ICurrentUser currentUser)
        {
            _context = context;
            _currentUser = currentUser;
            userGuid = Guid.Parse(_currentUser.UserId ?? throw new UserNotLoggedInException());
        }

        public async Task CreateStatusAsync(CreateStatusDto dto)
        {
            //deyisilmelidi
            var companyId = _context.Companies.FirstOrDefaultAsync(x => x.UserId == userGuid).Result.Id;

            var newStatus = new Status
            {
                IsDefault = false,
                StatusName = dto.StatusName.Trim(),
                CompanyId = companyId
            };

            await _context.Statuses.AddAsync(newStatus);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStatusAsync(string statusId)
        {
            var statusGuid = Guid.Parse(statusId);

            var existStatus = await _context.Statuses.FirstOrDefaultAsync(x => x.Id == statusGuid);

            if (existStatus == null) throw new SharedLibrary.Exceptions.NotFoundException<Status>();
            if (existStatus.IsDefault == true) throw new StatusPermissionException();

            _context.Statuses.Remove(existStatus);
            await _context.SaveChangesAsync();
        }

        public async Task<List<StatusListDto>> GetAllStatusesAsync()
        {
            //deyisilmelidi
            var companyId = _context.Companies.FirstOrDefaultAsync(x => x.UserId == userGuid).Result.Id;

            var allStatus = await _context.Statuses.Where(x => x.IsDefault == true && x.CompanyId == companyId)
            .Select(x => new StatusListDto
            {
                StatusId = x.Id,
                StatusName = x.StatusName,
            }).ToListAsync();

            return allStatus;
        }
    }
}