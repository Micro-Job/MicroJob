using Job.Business.Exceptions.Common;
using Job.Business.HelperServices.Current;
using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Dtos.VacancyDtos;
using Shared.Requests;
using Shared.Responses;
using SharedLibrary.Enums;
using SharedLibrary.Events;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using System.Security.Claims;

namespace Job.Business.Services.Vacancy
{
    //TODO : ümumilikde Bu vacancyService olmalı deyil Job-da
    public class VacancyService : IVacancyService
    {
        private readonly JobDbContext _context;
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
        readonly IConfiguration _configuration;
        private readonly string? _authServiceBaseUrl;
        private readonly ICurrentUser _currentUser;

        public VacancyService(
            JobDbContext context,
            IRequestClient<GetAllCompaniesRequest> request,
            IRequestClient<GetUserSavedVacanciesRequest> client,
            IHttpContextAccessor contextAccessor,
            IRequestClient<UserRegisteredEvent> requestClient,
            IRequestClient<GetAllVacanciesRequest> vacClient,
            IRequestClient<SimilarVacanciesRequest> similarRequest,
            IRequestClient<GetVacancyInfoRequest> vacancyInforRequest,
            IRequestClient<GetAllVacanciesByCompanyIdDataRequest> vacancyByCompanyId,
            IRequestClient<CheckVacancyRequest> checkVacancyRequest,
            IRequestClient<CheckCompanyRequest> checkCompanyRequest,
            IRequestClient<GetOtherVacanciesByCompanyRequest> othVacRequest,
            IConfiguration configuration,
            ICurrentUser currentUser
        )
        {
            _context = context;
            _request = request;
            _client = client;
            _contextAccessor = contextAccessor;
            _requestClient = requestClient;
            _vacClient = vacClient;
            _similarRequest = similarRequest;
            _vacancyInforRequest = vacancyInforRequest;
            _vacancyByCompanyId = vacancyByCompanyId;
            _checkVacancyRequest = checkVacancyRequest;
            _checkCompanyRequest = checkCompanyRequest;
            _othVacRequest = othVacRequest;
            _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
            _configuration = configuration;
            _currentUser = currentUser;
        }

        //TODO : savedVacancy buradan dasinmalidir - JobCompany-e ++++++++++
        /// <summary> Userin bütün save etdiyi vakansiyalarin get allu </summary>
        //public async Task<GetUserSavedVacanciesResponse> GetAllSavedVacancyAsync(int skip, int take)
        //{
        //    var savedVacanciesId = await _context
        //        .SavedVacancies.Where(x => x.UserId == _currentUser.UserGuid)
        //        .Select(x => x.VacancyId)
        //        .ToListAsync();

        //    var datas = await GetUserSavedVacancyDataAsync(savedVacanciesId, skip, take);

        //    return new GetUserSavedVacanciesResponse
        //    {
        //        Vacancies = datas.Vacancies,
        //        TotalCount = datas.TotalCount,
        //    };
        //}

        /// <summary> Consumer metodu -  Vacancy idlerine göre saved olunan vakansiyalarin datasi </summary>
        //private async Task<GetUserSavedVacanciesResponse> GetUserSavedVacancyDataAsync(List<Guid> vacancyIds,int skip,int take)
        //{
        //    if (vacancyIds == null || vacancyIds.Count == 0)
        //        return new GetUserSavedVacanciesResponse { Vacancies = [], TotalCount = 0 };


        //    var response = await _client.GetResponse<GetUserSavedVacanciesResponse>(
        //        new GetUserSavedVacanciesRequest
        //        {
        //            VacancyIds = vacancyIds,
        //            Skip = skip,
        //            Take = take,
        //        }
        //    );

        //    return new GetUserSavedVacanciesResponse
        //    {
        //        Vacancies = response.Message.Vacancies,
        //        TotalCount = response.Message.TotalCount,
        //    };
        //}

        /// <summary>
        /// Şirkətə aid olan digər vakansiyaların gətirilməsi
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="currentVacancyId"></param>
        /// <returns></returns>
        //public async Task<PaginatedVacancyDto> GetOtherVacanciesByCompanyAsync(string companyId,string? currentVacancyId,int skip = 1,int take = 6)
        //{
        //    var guidCompanyId = Guid.Parse(companyId);
        //    Guid? guidVacancyId = string.IsNullOrEmpty(currentVacancyId)
        //        ? null
        //        : Guid.Parse(currentVacancyId);

        //    await EnsureCompanyExistsAsync(guidCompanyId);

        //    if (guidVacancyId.HasValue)
        //        await EnsureVacancyExistsAsync(guidVacancyId.Value);

        //    var request = new GetOtherVacanciesByCompanyRequest
        //    {
        //        CompanyId = guidCompanyId,
        //        CurrentVacancyId = guidVacancyId,
        //        Skip = skip,
        //        Take = take,
        //    };

        //    var response = await _othVacRequest.GetResponse<GetOtherVacanciesByCompanyResponse>(
        //        request
        //    );


        //    if (_currentUser.UserGuid != Guid.Empty)
        //    {
        //        //var savedVacancies = await FetchUserSavedVacancyIdsAsync((Guid)_currentUser.UserGuid);
        //        MarkSavedVacancies(response.Message.Vacancies, savedVacancies);
        //    }

        //    return new PaginatedVacancyDto
        //    {
        //        Vacancies = response.Message.Vacancies,
        //        TotalCount = response.Message.TotalCount,
        //    };
        //}

        /// <summary>
        /// Vacancy detail-də şirket haqqında
        /// </summary>
        /// <param name="vacancyId"></param>
        /// <returns></returns>
        //public async Task<GetVacancyInfoResponse> GetVacancyInfoAsync(Guid vacancyId)
        //{
        //    var request = new GetVacancyInfoRequest { Id = vacancyId };
        //    var response = await _vacancyInforRequest.GetResponse<GetVacancyInfoResponse>(request);

        //    var userGuid = _currentUser.UserGuid;

        //    var savedVacancies =
        //        userGuid == null
        //            ? []
        //            : await _context
        //                .SavedVacancies.Where(x => x.UserId == userGuid)
        //                .AsNoTracking()
        //                .Select(x => x.VacancyId)
        //                .ToListAsync();

        //    response.Message.IsSaved = userGuid != null && savedVacancies.Contains(vacancyId);

        //    return response.Message;
        //}

        /// <summary> Butun vakansiyalarin getirilmesi - search ve filter</summary>
        //public async Task<PaginatedVacancyDto> GetAllVacanciesAsync(
        //    string? titleName,
        //    string? categoryId,
        //    string? countryId,
        //    string? cityId,
        //    bool? isActive,
        //    decimal? minSalary,
        //    decimal? maxSalary,
        //    string? companyId,
        //    WorkType? workType,
        //    WorkStyle? workStyle,
        //    int skip = 1,
        //    int take = 6
        //)
        //{
        //    var request = new GetAllVacanciesRequest
        //    {
        //        TitleName = titleName,
        //        CategoryId = categoryId,
        //        CountryId = countryId,
        //        CityId = cityId,
        //        IsActive = isActive,
        //        MinSalary = minSalary,
        //        MaxSalary = maxSalary,
        //        CompanyId = companyId,
        //        WorkType = workType,
        //        WorkStyle = workStyle,
        //    };

        //    var response = await _vacClient.GetResponse<GetAllVacanciesResponse>(request);
        //    Guid? userGuid = _currentUser.UserGuid;

        //    var savedVacancies =
        //        userGuid == Guid.Empty
        //            ? []
        //            : await _context
        //                .SavedVacancies.Where(x => x.UserId == userGuid)
        //                .Select(x => x.VacancyId)
        //                .ToListAsync();

        //    var savedVacanciesSet = new HashSet<Guid>(savedVacancies);

        //    var paginatedVacancies = response.Message.Vacancies.Skip((skip - 1) * take)
        //        .Take(take)
        //        .Select(x => new AllVacanyDto
        //        {
        //            VacancyId = x.VacancyId,
        //            CompanyName = x.CompanyName,
        //            Title = x.Title,
        //            CompanyLogo = x.CompanyLogo,
        //            StartDate = x.StartDate,
        //            Location = x.Location,
        //            MainSalary = x.MainSalary,
        //            ViewCount = x.ViewCount,
        //            IsVip = x.IsVip,
        //            IsActive = x.IsActive,
        //            CategoryId = x.CategoryId,
        //            WorkType = x.WorkType,
        //            WorkStyle = x.WorkStyle,
        //            IsSaved = userGuid != null && savedVacanciesSet.Contains(Guid.Parse(x.VacancyId)),
        //        })
        //        .ToList();


        //    return new PaginatedVacancyDto { Vacancies = paginatedVacancies, TotalCount = response.Message.TotalCount };
        //}


        //TODO : Oxsar vakansiyalarin getirilmesi neye gore Job proyektindedir / JobCompany-de olmalidir
        //Bu metotda optimization sohbetleri olmalidir
        /// <summary> Oxsar vakansiylarin getirilmesi category'e gore </summary>
        //public async Task<PaginatedSimilarVacancyDto> SimilarVacanciesAsync(string vacancyId)
        //{
        //    var userGuid = _currentUser.UserGuid;
        //    var guidVacancyId = Guid.Parse(vacancyId);

        //    //await EnsureVacancyExistsAsync(guidVacancyId);

        //    var savedVacancies =
        //        userGuid == Guid.Empty
        //            ? new List<Guid>()
        //            : await _context
        //                .SavedVacancies.Where(x => x.UserId == userGuid)
        //                .Select(x => x.VacancyId)
        //                .ToListAsync();

        //    var response = await _similarRequest.GetResponse<SimilarVacanciesResponse>(
        //        new SimilarVacanciesRequest { VacancyId = vacancyId }
        //    );

        //    var allVacancies = response
        //        .Message.Vacancies.Select(v => new SimilarVacancyDto
        //        {
        //            Id = v.Id,
        //            CompanyName = v.CompanyName,
        //            Title = v.Title,
        //            CompanyLogo = $"{_authServiceBaseUrl}/{v.CompanyPhoto}",
        //            StartDate = v.CreatedDate,
        //            Location = v.CompanyLocation,
        //            MainSalary = v.MainSalary,
        //            ViewCount = v.ViewCount,
        //            WorkType = v.WorkType,
        //            IsVip = v.IsVip,
        //            IsActive = v.IsActive,
        //            IsSaved = userGuid != Guid.Empty && savedVacancies.Contains(v.Id),
        //        })
        //        .ToList();

        //    return new PaginatedSimilarVacancyDto
        //    {
        //        SimilarVacancies = allVacancies,
        //        TotalCount = response.Message.TotalCount,
        //    };
        //}

        //TODO : Bu da burada olmali deyil(yeni ki bu proyektde)
        //public async Task<ICollection<AllVacanyDto>> GetAllVacanciesByCompanyId(string companyId)
        //{
        //    var guidCompanyId = Guid.Parse(companyId);

        //    var response =
        //        await _vacancyByCompanyId.GetResponse<GetAllVacanciesByCompanyIdDataResponse>(
        //            new GetAllVacanciesByCompanyIdDataRequest { CompanyId = guidCompanyId }
        //        );

        //    return response.Message.Vacancies;
        //}

        //private async Task EnsureVacancyExistsAsync(Guid vacancyId)
        //{
        //    var response = await _checkVacancyRequest.GetResponse<CheckVacancyResponse>(
        //        new CheckVacancyRequest { VacancyId = vacancyId }
        //    );

        //    if (!response.Message.IsExist)
        //        throw new EntityNotFoundException("Vacancy");
        //}

        //private async Task EnsureCompanyExistsAsync(Guid companyId)
        //{
        //    var response = await _checkCompanyRequest.GetResponse<CheckCompanyResponse>(
        //        new CheckCompanyRequest { CompanyId = companyId }
        //    );

        //    if (!response.Message.IsExist)
        //        throw new EntityNotFoundException("Company");
        //}

        //private static void MarkSavedVacancies(ICollection<AllVacanyDto> vacancies,ICollection<Guid> savedVacancies)
        //{
        //    foreach (var vacancy in vacancies)
        //    {
        //        vacancy.IsSaved = savedVacancies.Contains(Guid.Parse(vacancy.VacancyId));
        //    }
        //}

        //private async Task<ICollection<Guid>> FetchUserSavedVacancyIdsAsync(Guid userId)
        //{
        //    return await _context
        //        .SavedVacancies.Where(x => x.UserId == userId)
        //        .AsNoTracking()
        //        .Select(x => x.VacancyId)
        //        .ToListAsync();
        //}
    }
}
