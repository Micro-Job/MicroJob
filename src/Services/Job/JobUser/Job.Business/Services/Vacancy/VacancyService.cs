using AuthService.Business.Services.CurrentUser;
using Job.Core.Entities;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Business.Services.Vacancy
{
    public class VacancyService : IVacancyService
    {
        private readonly JobDbContext _context;
        private readonly ICurrentUser _currentUser;
        private readonly Guid userGuid;
        public VacancyService(JobDbContext context,ICurrentUser currentUser)
        {
            _context = context;
            _currentUser = currentUser;
            userGuid = Guid.Parse(_currentUser.UserId);
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

        public async Task GetAllSavedVacancyAsync()
        {
            var existSavedVacancy = await _context.SavedVacancies.Where(x => x.UserId == userGuid).Select(x => new 
            {
                Id = x.Id,
            }).ToListAsync();


        }
    }
}
