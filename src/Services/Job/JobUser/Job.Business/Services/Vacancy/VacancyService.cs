using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.CompanyDtos;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Job.Business.Services.Vacancy
{
    public class VacancyService : IVacancyService
    {
        private readonly JobDbContext _context;
        private readonly Guid userGuid;
        private readonly IRequestClient<GetAllCompaniesRequest> _request;
        private readonly IRequestClient<GetUserSavedVacanciesRequest> _client;
        private readonly IHttpContextAccessor _contextAccessor;

        public VacancyService(JobDbContext context,IRequestClient<GetAllCompaniesRequest> request, IRequestClient<GetUserSavedVacanciesRequest> client,IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _request = request;
            _client = client;
            _contextAccessor = contextAccessor;
            userGuid = Guid.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value);
        }

        /// <summary> Userin vakansiya save etme toggle metodu </summary>
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

        /// <summary> Userin bütün save etdiyi vakansiyalarin get allu </summary>
        public async Task<GetUserSavedVacanciesResponse> GetAllSavedVacancyAsync()
        {
            var savedVacanciesId = await _context.SavedVacancies
                .Where(x => x.UserId == userGuid)
                .Select(x => x.VacancyId)
                .ToListAsync();

            var datas = await GetUserSavedVacancyDataAsync(savedVacanciesId);

            return datas;
        }

        /// <summary> Bütün şirkətlərin get allu </summary>
        public async Task<ICollection<CompanyDto>> GetAllCompaniesAsync()
        {
            var response = await _request.GetResponse<GetAllCompaniesResponse>(new GetAllCompaniesRequest());

            return response.Message.Companies;
        }

        /// <summary> Consumer metodu -  Vacancy idlerine göre saved olunan vakansiyalarin datasi </summary>
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