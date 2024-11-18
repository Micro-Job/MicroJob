using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthService.Business.Services.CurrentUser;
using JobCompany.Business.Dtos.ApplicationDtos;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using SharedLibrary.Exceptions;

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

        public async Task UpdateApplicationAsync(string id,ApplicationUpdateDto dto)
        {
            var applicationId = Guid.Parse(id);
            var statusId = Guid.Parse(dto.StatusId);
            var existApplication = await _context.Applications.FindAsync(applicationId) 
            ?? throw new NotFoundException<Application>();

            existApplication.StatusId = statusId;
            await _context.SaveChangesAsync();
        }
    }
}
