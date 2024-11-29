using Job.Core.Entities;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary.Responses;
using SharedLibrary.Dtos.CompanyDtos;
using MassTransit;
using SharedLibrary.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Job.Business.Services.Vacancy
{
    public class VacancyService : IVacancyService
    {
        private readonly JobDbContext _context;
        private readonly Guid userGuid;
        private readonly IRequestClient<GetAllCompaniesRequest> _request;
        readonly IRequestClient<GetUserSavedVacanciesRequest> _client;
        private readonly IHttpContextAccessor _contextAccessor;

        public VacancyService(JobDbContext context,IRequestClient<GetAllCompaniesRequest> request, IRequestClient<GetUserSavedVacanciesRequest> client,IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _request = request;
            _client = client;
            _contextAccessor = contextAccessor;
            userGuid = Guid.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value);
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