using AuthService.Business.Services.CurrentUser;
using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.CompanyDtos;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Services.Vacancy
{
    public class VacancyService : IVacancyService
    {
        private readonly JobDbContext _context;
        private readonly ICurrentUser _currentUser;
        private readonly Guid userGuid;
        private readonly IRequestClient<GetAllCompaniesRequest> _request;
        readonly IRequestClient<GetUserSavedVacanciesRequest> _client;
        public VacancyService(JobDbContext context, ICurrentUser currentUser, IRequestClient<GetAllCompaniesRequest> request, IRequestClient<GetUserSavedVacanciesRequest> client)
        {
            _context = context;
            _currentUser = currentUser;
            userGuid = Guid.Parse(_currentUser.UserId);
            _request = request;
            _client = client;
        }

        public async Task ToggleSaveVacancyAsync(string vacancyId)
        {
            Guid vacancyGuid = Guid.Parse(vacancyId);
            var vacancyCheck = await _context.SavedVacancies.FirstOrDefaultAsync(x => x.VacancyId == vacancyGuid);

            if (vacancyCheck != null)
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

            var datas = await GetUserSavedVacancyDataAsync(savedVacanciesId);

            return datas;
        }

        public async Task<ICollection<CompanyDto>> GetAllCompaniesAsync()
        {
            var response = await _request.GetResponse<GetAllCompaniesResponse>(new GetAllCompaniesRequest());

            return response.Message.Companies;
        }


        public async Task<GetUserSavedVacanciesResponse> GetUserSavedVacancyDataAsync(List<Guid> vacancyIds)
        {
            if (vacancyIds == null || !vacancyIds.Any())
            {
                return new GetUserSavedVacanciesResponse
                {
                    Vacancies = new List<VacancyResponse>()
                };
            }

            var response = await _client.GetResponse<GetUserSavedVacanciesResponse>(
                new GetUserSavedVacanciesRequest { VacancyIds = vacancyIds }
            );

            return response.Message;
        }
    }
}