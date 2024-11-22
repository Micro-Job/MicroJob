using AuthService.Business.Services.CurrentUser;
using Job.Core.Entities;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary.Responses;
using Job.Business.Services.VacancyInformation;

namespace Job.Business.Services.Vacancy
{
    public class VacancyService : IVacancyService
    {
        private readonly JobDbContext _context;
        private readonly ICurrentUser _currentUser;
        private readonly Guid userGuid;
        private readonly IVacancyInformation _info;
        public VacancyService(JobDbContext context,ICurrentUser currentUser,IVacancyInformation info)
        {
            _context = context;
            _currentUser = currentUser;
            userGuid = Guid.Parse(_currentUser.UserId);
            _info = info;
        }

        public async Task ToggleSaveVacancyAsync(string vacancyId)
        {
            Guid vacancyGuid = Guid.Parse(vacancyId);
            var vacancyCheck = await _context.SavedVacancies.FirstOrDefaultAsync(x=>x.VacancyId == vacancyGuid);

            if(vacancyCheck != null)
            {
                _context.SavedVacancies.Remove(vacancyCheck);
            }
            else
            {
                await _context.SavedVacancies.AddAsync(new SavedVacancy
                {
                    UserId = userGuid,
                    VacancyId = vacancyGuid,
                });
            }
            await _context.SaveChangesAsync();
        }

        public async Task<GetUserSavedVacanciesResponse> GetAllSavedVacancyAsync()
        {
            var savedVacanciesId = await _context.SavedVacancies
                .Where(x => x.UserId == userGuid)
                .Select(x => x.VacancyId) 
                .ToListAsync();

            var salam = await _info.GetUserSavedVacancyDataAsync(savedVacanciesId);

            return salam;
        }
    }
}