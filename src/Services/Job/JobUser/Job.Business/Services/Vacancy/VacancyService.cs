using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos.VacancyDtos;
using Shared.Requests;
using Shared.Responses;
using SharedLibrary.Dtos.CompanyDtos;
using SharedLibrary.Dtos.VacancyDtos;
using SharedLibrary.Events;
using SharedLibrary.Exceptions;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using System.Security.Claims;

namespace Job.Business.Services.Vacancy
{
    public class VacancyService : IVacancyService
    {
        private readonly JobDbContext _context;
        private readonly Guid userGuid;
        private readonly IRequestClient<GetAllCompaniesRequest> _request;
        private readonly IRequestClient<GetUserSavedVacanciesRequest> _client;
        private readonly IRequestClient<GetAllVacanciesRequest> _vacClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRequestClient<UserRegisteredEvent> _requestClient;
        private readonly IRequestClient<SimilarVacanciesRequest> _similarRequest;

        public VacancyService(JobDbContext context, IRequestClient<GetAllCompaniesRequest> request, IRequestClient<GetUserSavedVacanciesRequest> client, IHttpContextAccessor contextAccessor, IRequestClient<UserRegisteredEvent> requestClient, IRequestClient<GetAllVacanciesRequest> vacClient, IRequestClient<SimilarVacanciesRequest> similarRequest)
        {
            _context = context;
            _request = request;
            _client = client;
            _contextAccessor = contextAccessor;
            userGuid = Guid.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value);
            _requestClient = requestClient;
            _vacClient = vacClient;
            _similarRequest = similarRequest;
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
            if (vacancyIds == null || vacancyIds.Count == 0)
            {
                return new GetUserSavedVacanciesResponse
                {
                    Vacancies = []
                };
            }

            var response = await _client.GetResponse<GetUserSavedVacanciesResponse>(
                new GetUserSavedVacanciesRequest { VacancyIds = vacancyIds }
            );

            return response.Message;
        }

        /// <summary>Company'e gore butun vakansiyalarin getirilmesi</summary>
        public async Task<List<VacancyDto>> GetAllUserVacanciesAsync()
        {
            var response = await _request.GetResponse<UserVacanciesResponse>(new GetAllUserVacanciesRequest());

            return response.Message.Vacancies;
        }
        /// <summary>
        /// Vacancy detail-də şirket haqqında
        /// </summary>
        /// <param name="vacancyId"></param>
        /// <returns></returns>
        public async Task<GetVacancyInfoResponse> GetVacancyInfoAsync(Guid vacancyId)
        {
            var response = await _vacancyClient.GetResponse<GetVacancyInfoResponse>(vacancyId);
            return response.Message;

        /// <summary> Butun vakansiyalarin getirilmesi - search ve filter</summary>
        public async Task<ICollection<AllVacanyDto>> GetAllVacanciesAsync(string? titleName, string? categoryId, string? countryId, string? cityId, bool? isActive, decimal? minSalary, decimal? maxSalary, int skip = 1, int take = 6)
        {
            var request = new GetAllVacanciesRequest
            {
                TitleName = titleName,
                CategoryId = categoryId,
                CountryId = countryId,
                CityId = cityId,
                IsActive = isActive,
                MinSalary = minSalary,
                MaxSalary = maxSalary,
            };

            var response = await _vacClient.GetResponse<GetAllVacanciesResponse>(request);

            var vacancies = response.Message.Vacancies.AsQueryable();

            var pagedVacancies = vacancies
                .Skip((skip - 1) * take)
                .Take(take)
                .ToList();

            return pagedVacancies;
        }

        /// <summary> Oxsar vakansiylarin getirilmesi category'e gore </summary>
        public async Task<List<SimilarVacancyResponse>> SimilarVacanciesAsync(string vacancyId, string userId)
        {
            var guidUserId = Guid.Parse(userId);
            var guidVacancyId = Guid.Parse(vacancyId);

            var savedVacancies = await _context.SavedVacancies
                .Where(sv => sv.UserId == guidUserId)
                .Select(sv => sv.VacancyId)
                .ToListAsync();

            var response = await _similarRequest.GetResponse<SimilarVacanciesResponse>(
                new SimilarVacanciesRequest { VacancyId = vacancyId }
            );

            var allVacancies = response.Message.Vacancies.Select(v => new SimilarVacancyResponse
            {
                CompanyName = v.CompanyName,
                Title = v.Title,
                CompanyPhoto = v.CompanyPhoto,
                CreatedDate = v.CreatedDate,
                CompanyLocation = v.CompanyLocation,
                MainSalary = v.MainSalary,
                ViewCount = v.ViewCount,
                WorkType = v.WorkType,
                IsVip = v.IsVip,
                IsActive = v.IsActive,
                CategoryId = v.CategoryId,
                IsSaved = savedVacancies.Contains(v.Id)
            }).ToList();

            return allVacancies;
        }
    }
}

