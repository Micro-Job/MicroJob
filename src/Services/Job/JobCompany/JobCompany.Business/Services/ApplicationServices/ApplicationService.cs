using JobCompany.Business.Dtos.ApplicationDtos;
using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.StatusDtos;
using JobCompany.Business.Exceptions.ApplicationExceptions;
using JobCompany.Business.Extensions;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Extensions;
using Shared.Events;
using Shared.Requests;
using Shared.Responses;
using SharedLibrary.Dtos.ApplicationDtos;
using SharedLibrary.Enums;
using SharedLibrary.Exceptions;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Services.ApplicationServices
{
    public class ApplicationService : IApplicationService
    {
        private readonly JobCompanyDbContext _context;
        readonly IRequestClient<GetUsersDataRequest> _getUserDataClient;
        readonly IRequestClient<GetResumeDataRequest> _getResumeDataClient;
        readonly IPublishEndpoint _publishEndpoint;
        readonly IConfiguration _configuration;
        private readonly ICurrentUser _currentUser;
        private readonly string? _authServiceBaseUrl;
        private readonly IRequestClient<GetUserDataRequest> _requestUser;

        public ApplicationService(
            JobCompanyDbContext context,
            IRequestClient<GetUsersDataRequest> client,
            IRequestClient<GetResumeDataRequest> requestClient,
            IPublishEndpoint publishEndpoint,
            IConfiguration configuration,
            ICurrentUser currentUser,
            IRequestClient<GetUserDataRequest> requestUser)
        {
            _currentUser = currentUser;
            _context = context;
            _getUserDataClient = client;
            _getResumeDataClient = requestClient;
            _publishEndpoint = publishEndpoint;
            _configuration = configuration;
            _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
            _requestUser = requestUser;
        }

        /// <summary> Yaradılan müraciətin geri alınması </summary>
        public async Task RemoveApplicationAsync(string applicationId)
        {
            var applicationGuid = Guid.Parse(applicationId);

            var existApplication =
                await _context.Applications.FirstOrDefaultAsync(x =>
                    x.Id == applicationGuid && x.UserId == _currentUser.UserGuid
                ) ?? throw new NotFoundException<Application>(MessageHelper.GetMessage("NOT_FOUND"));
            if (existApplication.IsActive == false)
                throw new ApplicationStatusIsDeactiveException(MessageHelper.GetMessage("APPLICATION_IS_DEACTIVE"));
            existApplication.IsActive = false;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Müraciətin statusunun dəyişilməsi ve eventle usere bildiris publishi
        /// </summary>
        public async Task ChangeApplicationStatusAsync(string applicationId, string statusId)
        {
            var statusGuid = Guid.Parse(statusId);
            var applicationGuid = Guid.Parse(applicationId);

            var existAppVacancy = await _context.Applications
                .Where(x =>x.Id == applicationGuid && x.Vacancy.Company.UserId == _currentUser.UserGuid)
                .Select(x => new
                {
                    Application = x,
                    VacancyId = x.VacancyId
                })
                .FirstOrDefaultAsync()
            ?? throw new NotFoundException<Application>(MessageHelper.GetMessage("NOT_FOUND"));

            var application = existAppVacancy.Application;

            application.StatusId = statusGuid;
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(
                new UpdateUserApplicationStatusEvent
                {
                    UserId = application.UserId,
                    SenderId = (Guid)_currentUser.UserGuid,
                    InformationId = existAppVacancy.VacancyId,
                    //Content =
                    //    $"{vacancy.CompanyName} şirkətinin müraciət statusu dəyişdirildi: {application.Status.Name}",
                }
            );
        }

        /// <summary> Müraciətlərin statusu ilə birlikdə gətirilməsi </summary>
        public async Task<List<StatusListDtoWithApps>> GetAllApplicationWithStatusAsync(string vacancyId)
        {
            var vacancyGuid = Guid.Parse(vacancyId);

            var groupedData = await _context
                .Statuses.Where(status => status.Company.UserId == _currentUser.UserGuid)
                .Select(status => new StatusListDtoWithApps
                {
                    StatusId = status.Id,
                    StatusName = status.GetTranslation(_currentUser.LanguageCode),
                    StatusColor = status.StatusColor,
                    IsDefault = status.IsDefault,
                    Applications = _context
                        .Applications.Where(app =>
                            app.VacancyId == vacancyGuid
                            && app.IsActive
                            && app.StatusId == status.Id
                        )
                        .Take(5)
                        .Select(app => new ApplicationListDto
                        {
                            ApplicationId = app.Id,
                            UserId = app.UserId,
                            VacancyId = app.VacancyId,
                            IsActive = app.IsActive,
                        })
                        .ToList(),
                })
                .ToListAsync();

            return groupedData;
        }

        /// <summary> Id'yə görə müraciətin gətirilməsi </summary>
        public async Task<ApplicationGetByIdDto> GetApplicationByIdAsync(string applicationId)
        {
            var applicationGuid = Guid.Parse(applicationId);

            var application = await _context.Applications
                    .Where(a => a.Id == applicationGuid && a.IsActive)
                    .Select(a => new ApplicationGetByIdDto
                    {
                        VacancyId = a.VacancyId,
                        VacancyImage = a.Vacancy.CompanyLogo,
                        VacancyTitle = a.Vacancy.Title,
                        CompanyName = a.Vacancy.CompanyName,
                        CreatedDate = a.CreatedDate,
                        Description = a.Vacancy.Description,

                        StatusName = a.Status.GetTranslation(_currentUser.LanguageCode),
                        StatusColor = a.Status.StatusColor,
                        Steps = _context
                            .Statuses.OrderBy(s => s.Order)
                            .Select(s => s.GetTranslation(_currentUser.LanguageCode))
                            .ToList(),
                    })
                    .FirstOrDefaultAsync() ?? throw new NotFoundException<Application>(MessageHelper.GetMessage("NOT_FOUND"));

            return application;
        }

        /// <summary> Consumer metodu - user idlərinə görə user datalarının gətirilməsi </summary>
        private async Task<GetUsersDataResponse> GetUserDataResponseAsync(List<Guid> userIds)
        {
            var request = new GetUsersDataRequest { UserIds = userIds };

            var response = await _getUserDataClient.GetResponse<GetUsersDataResponse>(request);

            var userDataResponse = new GetUsersDataResponse { Users = response.Message.Users };

            return userDataResponse;
        }

        /// <summary> Consumer metodu - Useridlere görə resumelerin getirilmesi </summary>
        private async Task<GetResumesDataResponse> GetResumeDataResponseAsync(List<Guid> userIds)
        {
            var request = new GetResumeDataRequest { UserIds = userIds };

            var response = await _getResumeDataClient.GetResponse<GetResumesDataResponse>(request);

            var userDataResponse = new GetResumesDataResponse { Users = response.Message.Users };

            return userDataResponse;
        }

        /// <summary> Daxil olmus muracietler -> Şirkət üçün bütün müraciətlərin gətirilməsi </summary>
        public async Task<ICollection<ApplicationInfoListDto>> GetAllApplicationAsync(int skip = 1,int take = 9)
        {
            var company =
                await _context.Companies.FirstOrDefaultAsync(c => c.UserId == _currentUser.UserGuid)
                ?? throw new NotFoundException<Company>(MessageHelper.GetMessage("NOT_FOUND"));
            var applications = await _context
                .Applications.Include(a => a.Vacancy)
                .ThenInclude(v => v.Company)
                .Where(a => a.Vacancy.Company.UserId == _currentUser.UserGuid)
                .Select(a => new ApplicationInfoDto
                {
                    ApplicationId = a.Id,
                    UserId = a.UserId,
                    CreatedDate = a.CreatedDate,
                })
                .ToListAsync();

            var userIds = applications.Select(a => a.UserId).ToList();

            var userDataResponse = await GetUserDataResponseAsync(userIds);
            var userResumeResponse = await GetResumeDataResponseAsync(userIds);

            var applicationList = applications
                .GroupBy(a => a.UserId)
                .SelectMany(group =>
                {
                    var userData = userDataResponse.Users.FirstOrDefault(u =>
                        u.UserId == group.Key
                    );
                    var userResume = userResumeResponse.Users.FirstOrDefault(r =>
                        r.UserId == group.Key
                    );

                    return group.Select(application => new ApplicationInfoListDto
                    {
                        ApplicationId = application.ApplicationId,
                        FullName = $"{userData?.FirstName} {userData?.LastName}",
                        ImageUrl = $"{_authServiceBaseUrl}/{userData?.ProfileImage}",
                        Position = userResume?.Position,
                        CreatedDate = application.CreatedDate,
                    });
                })
                .Skip((skip - 1) * take)
                .Take(take)
                .ToList();

            return applicationList;
        }

        /// <summary> Şirkətə daxil olan bütün müraciətlərin filterlə birlikdə detallı şəkildə gətirilməsi </summary>
        public async Task<DataListDto<AllApplicationListDto>> GetAllApplicationsListAsync(int skip = 1,int take = 10)
        {
            var applications = await GetPaginatedApplicationsAsync(skip, take);

            var userIds = applications.Item1.Select(a => a.UserId).ToList();

            var userDataResponse = await GetUserDataResponseAsync(userIds);

            var data = MapApplicationsToDto(applications.Item1, userDataResponse);

            return new DataListDto<AllApplicationListDto> 
            { 
                Datas = data,
                TotalCount = applications.Item2
            };
        }

        private List<AllApplicationListDto> MapApplicationsToDto(List<Application> applications,GetUsersDataResponse usersDataResponse)
        {
            var response = applications
                .Select(a =>
                {
                    var user = usersDataResponse.Users.FirstOrDefault(u => u.UserId == a.UserId);

                    return new AllApplicationListDto()
                    {
                        ApplicationId = a.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        StatusName = a.Status.GetTranslation(_currentUser.LanguageCode),
                        VacancyId = a.VacancyId,
                        VacancyName = a.Vacancy.Title,
                    };
                })
                .ToList();

            return response;
        }

        private async Task<(List<Application> , int)> GetPaginatedApplicationsAsync(int skip, int take)
        {
            var query = _context.Applications
                .Include(a => a.Vacancy)
                .Include(a => a.Status)
                .Where(a => a.Vacancy.Company.UserId == _currentUser.UserGuid)
                .OrderByDescending(a => a.CreatedDate);

            var applications = await query
                .Skip(Math.Max(0, (skip - 1) * take))
                .Take(take)
                .ToListAsync();

            return (applications , await query.CountAsync());
        }

        public async Task CreateUserApplicationAsync(string vacancyId)
        {
            var userGuid = _currentUser.UserGuid ?? throw new Exception(MessageHelper.GetMessage("NOT_FOUND"));

            var vacancyGuid = Guid.Parse(vacancyId);

            if (await _context.Applications.AnyAsync(x=> x.VacancyId == vacancyGuid && x.UserId == userGuid)) 
                throw new ApplicationIsAlreadyExistException(MessageHelper.GetMessage("APPLICATION_ALREADY_EXIST"));

            var vacancyInfo = await _context.Vacancies
                .Where(v => v.Id == vacancyGuid)
                .Select(v => new { v.CompanyId, v.Title })
                .FirstOrDefaultAsync() ?? throw new NotFoundException<Company>("NOT_FOUND");

            var status = await _context.Statuses.FirstOrDefaultAsync(s => s.StatusEnum == StatusEnum.Pending && s.IsDefault);

            var responseUser = await _requestUser.GetResponse<GetUserDataResponse>(
                new GetUserDataRequest { UserId = userGuid }
            );

            var newApplication = new Application
            {
                UserId = userGuid,
                VacancyId = vacancyGuid,
                StatusId = status.Id,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            await _context.Applications.AddAsync(newApplication);
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(new VacancyApplicationEvent
            {
                UserId = (Guid)vacancyInfo.CompanyId,
                SenderId = userGuid,
                VacancyId = vacancyGuid,
                InformationId = userGuid,
                Content = $"İstifadəçi {responseUser.Message.FirstName} {responseUser.Message.LastName} {vacancyInfo.Title} vakansiyasına müraciət etdi.",
            });
        }

        public async Task<PaginatedApplicationDto> GetUserApplicationsAsync(int skip, int take)
        {
            var query = _context.Applications
                .Where(a => a.UserId == _currentUser.UserGuid && a.IsActive);

            int totalCount = await query.CountAsync();

            var applications = await query
                .Include(x=> x.Status)
                    .ThenInclude(x=>x.Translations)
                .OrderByDescending(a => a.CreatedDate)
                .Select(a => new ApplicationDto
                {
                    ApplicationId = a.Id,
                    VacancyId = a.VacancyId,
                    Title = a.Vacancy.Title,
                    CompanyId = a.Vacancy.CompanyId,    
                    CompanyLogo = a.Vacancy.Company.CompanyLogo,
                    CompanyName = a.Vacancy.Company.CompanyName,
                    WorkType = a.Vacancy.WorkType != null ? a.Vacancy.WorkType.GetDisplayName() : null, 
                    IsActive = a.IsActive,
                    StatusName = a.Status.IsDefault ? a.Status.GetTranslation(_currentUser.LanguageCode) : a.Status.Translations.FirstOrDefault().Name,
                    StatusColor = a.Status.StatusColor,
                    ViewCount = a.Vacancy.ViewCount,
                    StartDate = a.CreatedDate,
                    MainSalary = a.Vacancy.MainSalary,
                    MaxSalary = a.Vacancy.MaxSalary
                })
                .Skip(Math.Max(0, (skip - 1) * take))
                .Take(take)
                .ToListAsync();

            return new PaginatedApplicationDto
            {
                Applications = applications,
                TotalCount = totalCount
            };
        }

        public async Task<ApplicationDetailDto> GetUserApplicationByIdAsync(string applicationId)
        {
            var applicationGuid = Guid.Parse(applicationId);
    
            var application = await _context.Applications
                .Include(x=> x.Status)
                    .ThenInclude(x=> x.Translations)
                .Where(a => a.UserId == _currentUser.UserGuid && a.Id == applicationGuid)
                .Select(application => new ApplicationDetailDto
                {
                    VacancyId = application.VacancyId,
                    VacancyName = application.Vacancy.Title,
                    CompanyId = application.Vacancy.CompanyId,
                    CompanyName = application.Vacancy.Company.CompanyName,
                    CompanyLogo =  $"{_currentUser.BaseUrl}/{application.Vacancy.Company.CompanyLogo}",
                    Location = application.Vacancy.Company.CompanyLocation,
                    WorkType = application.Vacancy.WorkType,
                    WorkStyle = application.Vacancy.WorkStyle,
                    CreatedDate = application.CreatedDate,
                    ApplicationStatusId = application.StatusId,
                    ApplicationStatusName = application.Status.IsDefault ? application.Status.GetTranslation(_currentUser.LanguageCode) : application.Status.Translations.FirstOrDefault().Name
                })
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException<Application>(MessageHelper.GetMessage("NOT_FOUND"));
                
            application.CompanyStatuses = await _context.Statuses.Where(x => x.CompanyId == application.CompanyId || x.IsDefault)
                .Include(x => x.Translations)
                .OrderBy(x => x.Order)
                .Select(x => new ApplicationStatusesListDto
                {
                    CompanyStatusId = x.Id,
                    CompanyStatusName = x.IsDefault ? x.GetTranslation(_currentUser.LanguageCode) : x.Translations.FirstOrDefault().Name,
                    Order = x.Order
                })
                .ToListAsync();

            return application;
        }

    }
}