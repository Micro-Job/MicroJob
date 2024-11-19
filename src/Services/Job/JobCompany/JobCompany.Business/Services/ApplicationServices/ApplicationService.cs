using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthService.Business.Services.CurrentUser;
using JobCompany.Business.Dtos.ApplicationDtos;
using JobCompany.Business.Dtos.StatusDtos;
using JobCompany.Business.Exceptions.StatusExceptions;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Exceptions;
using SharedLibrary.Middlewares;

namespace JobCompany.Business.Services.ApplicationServices
{
    public class ApplicationService : IApplicationService
    {
        private readonly JobCompanyDbContext _context;
        private readonly ICurrentUser _currentUser;
        private readonly Guid userGuid;

        public ApplicationService( ICurrentUser currentUser, JobCompanyDbContext context)
        {
            _context = context;
            _currentUser = currentUser;
            userGuid = Guid.Parse(_currentUser.UserId);
        }

        public async Task CreateApplicationAsync(ApplicationCreateDto dto)
        {
            var VacancyId = Guid.Parse(dto.VacancyId);
            var vacancy = await _context.Vacancies.FindAsync(dto.VacancyId)
            ?? throw new NotFoundException<Vacancy>();

            var application = new Application
            {
                UserId = userGuid, 
                VacancyId = VacancyId, 
                IsActive = true  
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeApplicationStatusAsync(string applicationId, string statusId)
        {
            var statusGuid = Guid.Parse(statusId);
            var applicationGuid = Guid.Parse(applicationId);

            var existAppVacancy = await _context.Applications
                                                .Include(x=>x.Vacancy)
                                                    .ThenInclude(x=>x.Company).FirstOrDefaultAsync(x=>x.Id == applicationGuid) 
                ?? throw new NotFoundException<Application>("Müraciət mövcud deyil!");

            if (existAppVacancy.Vacancy.Company.UserId != userGuid) throw new StatusPermissionException();

            existAppVacancy.StatusId = statusGuid;
            await _context.SaveChangesAsync();
        }

        public async Task<List<StatusListDtoWithApps>> GetAllApplicationAsync(string vacancyId)
        {
            var vacancyGuid = Guid.Parse(vacancyId);

            var statuses = await _context.Statuses.Where(x=> x.Company.UserId == userGuid || x.IsDefault == true).ToListAsync();

            var applications = await _context.Applications.Where(x => x.VacancyId == vacancyGuid && x.IsActive == true).ToListAsync();

            var groupedData = statuses.Select(status => new StatusListDtoWithApps
            {
                StatusId = status.Id,
                StatusName = status.StatusName,
                StatusColor = status.StatusColor,
                IsDefault = status.IsDefault,
                Applications = applications
            .Where(app => app.StatusId == status.Id)
                .Select(app => new ApplicationListDto
                {
                    ApplicationId = app.Id,
                    UserId = app.UserId,
                    VacancyId = app.VacancyId,
                    IsActive = app.IsActive
                }).Take(5).ToList()
            }).ToList();

            return groupedData;
        }

        //bu metodun return-ü olmalıdır
        public async Task GetAllApplicationWithStatusAsync(string vacancyId, string statusId, int skip = 1, int take = 5)
        {
            var vacancyGuid = Guid.Parse(vacancyId);
            var statusGuid = Guid.Parse(statusId);

            var applications = await _context.Applications.Where(x=>x.VacancyId == vacancyGuid && x.StatusId == statusGuid).Select(x=> new
            {
                x.StatusId,
                x.UserId,
            })
            .Skip(Math.Max(0,(skip - 1) * take))
            .Take(take)
            .ToListAsync();
        }

        public async Task<List<ApplicationUserListDto>> GetUserApplicationAsync(int? skip = 1, int take = 9)
        {
            var userApps = await _context.Applications.Where(x=>x.UserId == userGuid).Select().Skip(Math.Max(0,(skip - 1)* take)).Take(take).ToListAsync();   
        }
    }
}
