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
        //TODO : Burada status silindiyi zaman ilk once yoxlamaliyiq ki bu sildiyi status ozunundur mu.Belke basqa bir company-nin statusunu silir.
        //2cisi burada tam olaraq baxmadim boyuk ehtimal burada relationda DeleteBehavior.Resctrict verilib yeni eger bu statusla bagli application varsa sile bilmesin yeni bele bir exception qaytarmaliyiq
        public async Task DeleteStatusAsync(string statusId)
        {
            var statusGuid = Guid.Parse(statusId);

            var existStatus = await _context.Statuses.FirstOrDefaultAsync(x => x.Id == statusGuid && x.Company.UserId == userGuid)
                ?? throw new SharedLibrary.Exceptions.NotFoundException<Status>();

            if (existStatus.IsDefault == true) throw new StatusPermissionException();

            _context.Statuses.Remove(existStatus);
            await _context.SaveChangesAsync();
        }

        //TODO : Burada notificationdaki kimi yene sql-e 2 defe gedilir birbasa statusdan gedile biler.2cisi
        //bu metodu isletmedim ama oxudugum qederile islemeyecek.Cunki sertde deyilib ki defaultu true olan ve 
        //companyId si bu olan bu ola bilmez cunki defaultu biz yaradiriq ve burada demisik ki companyIdsi olan amma bizim olan
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