using Job.Business.Exceptions.Common;
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
        private readonly IRequestClient<GetVacancyInfoRequest> _vacancyInforRequest;
        private readonly IRequestClient<GetAllVacanciesByCompanyIdDataRequest> _vacancyByCompanyId;
        private readonly IRequestClient<CheckVacancyRequest> _checkVacancyRequest;
        private readonly IRequestClient<CheckCompanyRequest> _checkCompanyRequest;
        private readonly IRequestClient<GetOtherVacanciesByCompanyRequest> _othVacRequest;

        public VacancyService(JobDbContext context, IRequestClient<GetAllCompaniesRequest> request, IRequestClient<GetUserSavedVacanciesRequest> client, IHttpContextAccessor contextAccessor,
            IRequestClient<UserRegisteredEvent> requestClient, IRequestClient<GetAllVacanciesRequest> vacClient, IRequestClient<SimilarVacanciesRequest> similarRequest,
            IRequestClient<GetVacancyInfoRequest> vacancyInforRequest, IRequestClient<GetAllVacanciesByCompanyIdDataRequest> vacancyByCompanyId, IRequestClient<CheckVacancyRequest> checkVacancyRequest,
            IRequestClient<CheckCompanyRequest> checkCompanyRequest, IRequestClient<GetOtherVacanciesByCompanyRequest> othVacRequest)
        {
            _context = context;
            _request = request;
            _client = client;
            _contextAccessor = contextAccessor;
            userGuid = Guid.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value);
            _requestClient = requestClient;
            _vacClient = vacClient;
            _similarRequest = similarRequest;
            _vacancyInforRequest = vacancyInforRequest;
            _vacancyByCompanyId = vacancyByCompanyId;
            _checkVacancyRequest = checkVacancyRequest;
            _checkCompanyRequest = checkCompanyRequest;
            _othVacRequest = othVacRequest;
        }

        /// <summary> Userin vakansiya save etme toggle metodu </summary>
        public async Task ToggleSaveVacancyAsync(string vacancyId)
        {
            Guid vacancyGuid = Guid.Parse(vacancyId);

            await EnsureVacancyExistsAsync(vacancyGuid);

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
        private async Task<GetUserSavedVacanciesResponse> GetUserSavedVacancyDataAsync(List<Guid> vacancyIds)
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

        /// <summary>
        /// Şirkətə aid olan digər vakansiyaların gətirilməsi
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="currentVacancyId"></param>
        /// <returns></returns>
        public async Task<ICollection<AllVacanyDto>> GetOtherVacanciesByCompanyAsync(string companyId, string currentVacancyId)
        {
            var guidCompanyId = Guid.Parse(companyId);
            var guidVacancyId = Guid.Parse(currentVacancyId);

            await EnsureCompanyExistsAsync(guidCompanyId);

            await EnsureVacancyExistsAsync(guidVacancyId);

            var request = new GetOtherVacanciesByCompanyRequest
            {
                CompanyId = guidCompanyId,
                CurrentVacancyId = guidVacancyId
            };

            var response = await _othVacRequest.GetResponse<GetOtherVacanciesByCompanyResponse>(request);

            return response.Message.Vacancies ?? [];
        }

        /// <summary>
        /// Vacancy detail-də şirket haqqında
        /// </summary>
        /// <param name="vacancyId"></param>
        /// <returns></returns>
        public async Task<GetVacancyInfoResponse> GetVacancyInfoAsync(Guid vacancyId)
        {
            var request = new GetVacancyInfoRequest { Id = vacancyId };
            var response = await _vacancyInforRequest.GetResponse<GetVacancyInfoResponse>(request);
            return response.Message;
        }
        /// <summary> Butun vakansiyalarin getirilmesi - search ve filter</summary>
        public async Task<ICollection<AllVacanyDto>> GetAllVacanciesAsync(string? titleName, string? categoryId, string? countryId, string? cityId, bool? isActive, decimal? minSalary, decimal? maxSalary, int skip = 1, int take = 6)
        {
            var userSkills = await _context.Resumes
            .Where(r => r.UserId == userGuid)
            .SelectMany(r => r.ResumeSkills)
            .ToListAsync();

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
        public async Task<ICollection<SimilarVacancyDto>> SimilarVacanciesAsync(string vacancyId)
        {
            var guidVacancyId = Guid.Parse(vacancyId);

            await EnsureVacancyExistsAsync(guidVacancyId);

            var savedVacancies = await _context.SavedVacancies
                .Where(sv => sv.UserId == userGuid)
                .Select(sv => sv.VacancyId)
                .ToListAsync();

            var response = await _similarRequest.GetResponse<SimilarVacanciesResponse>(
                new SimilarVacanciesRequest { VacancyId = vacancyId }
            );

            var allVacancies = response.Message.Vacancies.Select(v => new SimilarVacancyDto
            {
                CompanyName = v.CompanyName,
                Title = v.Title,
                CompanyLogo = v.CompanyPhoto,
                StartDate = v.CreatedDate,
                Location = v.CompanyLocation,
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

        public async Task<ICollection<AllVacanyDto>> GetAllVacanciesByCompanyId(string companyId)
        {
            var guidCompanyId = Guid.Parse(companyId);

            var response = await _vacancyByCompanyId.GetResponse<GetAllVacanciesByCompanyIdDataResponse>(
                new GetAllVacanciesByCompanyIdDataRequest { CompanyId = guidCompanyId }
            );

            return response.Message.Vacancies;
        }

        private async Task EnsureVacancyExistsAsync(Guid vacancyId)
        {
            var response = await _checkVacancyRequest.GetResponse<CheckVacancyResponse>(new CheckVacancyRequest
            {
                VacancyId = vacancyId
            });

            if (!response.Message.IsExist) throw new EntityNotFoundException("Vacancy");
        }

        private async Task EnsureCompanyExistsAsync(Guid companyId)
        {
            var response = await _checkCompanyRequest.GetResponse<CheckCompanyResponse>(new CheckCompanyRequest
            {
                CompanyId = companyId
            });

            if (!response.Message.IsExist) throw new EntityNotFoundException("Company");
        }
    }
}