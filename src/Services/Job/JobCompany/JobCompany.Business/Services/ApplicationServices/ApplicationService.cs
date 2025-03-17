using JobCompany.Business.Dtos.ApplicationDtos;
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
using Shared.Events;
using Shared.Requests;
using Shared.Responses;
using SharedLibrary.Dtos.ApplicationDtos;
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
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _baseUrl;
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
            IHttpContextAccessor contextAccessor,
            IPublishEndpoint publishEndpoint,
            IConfiguration configuration,
            ICurrentUser currentUser,
            IRequestClient<GetUserDataRequest> requestUser)
        {
            _currentUser = currentUser;
            _context = context;
            _contextAccessor = contextAccessor;
            _getUserDataClient = client;
            _getResumeDataClient = requestClient;
            _baseUrl =
                $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host.Value}{_contextAccessor.HttpContext.Request.PathBase.Value}";
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

            var existAppVacancy =
                await _context
                    .Applications.Where(x =>
                        x.Id == applicationGuid && x.Vacancy.Company.UserId == _currentUser.UserGuid
                    )
                    .Select(x => new
                    {
                        Application = x,
                        Vacancy = new
                        {
                            x.Vacancy.Id,
                            x.Vacancy.CompanyId,
                            CompanyName = x.Vacancy.Company.CompanyName,
                        },
                    })
                    .FirstOrDefaultAsync()
                ?? throw new NotFoundException<Application>(MessageHelper.GetMessage("NOT_FOUND"));

            var application = existAppVacancy.Application;
            var vacancy = existAppVacancy.Vacancy;

            application.StatusId = statusGuid;
            await _context.SaveChangesAsync();

            await _context.Entry(application).Reference(x => x.Status).LoadAsync();

            await _publishEndpoint.Publish(
                new UpdateUserApplicationStatusEvent
                {
                    UserId = application.UserId,
                    SenderId = (Guid)_currentUser.UserGuid,
                    InformationId = vacancy.Id,
                    //Content =
                    //    $"{vacancy.CompanyName} şirkətinin müraciət statusu dəyişdirildi: {application.Status.Name}",
                }
            );
        }

        /// <summary> Müraciətlərin statusu ilə birlikdə gətirilməsi </summary>
        public async Task<List<StatusListDtoWithApps>> GetAllApplicationWithStatusAsync(
            string vacancyId
        )
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

        /// <summary> Userin müraciətlərinin gətirilməsi </summary>
        public async Task<List<ApplicationUserListDto>> GetUserApplicationAsync(int skip = 1,int take = 9)
        {
            var userApps = await _context
                .Applications.Include(x => x.Status.Translations).Where(x => x.UserId == _currentUser.UserGuid)
                .Select(x => new ApplicationUserListDto
                {
                    CompanyName = x.Vacancy.Company.CompanyName,
                    CreatedDate = x.CreatedDate,
                    MainSalary = x.Vacancy.MainSalary,
                    MaxSalary = x.Vacancy.MaxSalary,
                    StatusColor = x.Status.StatusColor,
                    StatusName = x.Status.GetTranslation(_currentUser.LanguageCode),
                    VacancyId = x.VacancyId,
                    VacancyImage = $"{_baseUrl}/{x.Vacancy.CompanyLogo}",
                    VacancyTitle = x.Vacancy.Title,
                    ViewCount = x.Vacancy.ViewCount,
                })
                .Skip(Math.Max(0, (skip - 1) * take))
                .Take(take)
                .ToListAsync();

            return userApps;
        }

        /// <summary> Id'yə görə müraciətin gətirilməsi </summary>
        public async Task<ApplicationGetByIdDto> GetApplicationByIdAsync(string applicationId)
        {
            var applicationGuid = Guid.Parse(applicationId);

            var application =
                await _context
                    .Applications.Include(a => a.Status.Translations).Where(a => a.Id == applicationGuid && a.IsActive)
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
        public async Task<ICollection<AllApplicationListDto>> GetAllApplicationsListAsync(int skip = 1,int take = 10)
        {
            var applications = await GetPaginatedApplicationsAsync(skip, take);

            var userIds = applications.Select(a => a.UserId).ToList();

            var userDataResponse = await GetUserDataResponseAsync(userIds);

            return MapApplicationsToDto(applications, userDataResponse);
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

        private async Task<List<Application>> GetPaginatedApplicationsAsync(int skip, int take)
        {
            var applications =await _context.Applications
                .Include(a => a.Vacancy)
                .Include(a => a.Status)
                .Where(a => a.Vacancy.Company.UserId == _currentUser.UserGuid)
                .OrderByDescending(a => a.CreatedDate)
                .Skip(Math.Max(0, (skip - 1) * take))
                .Take(take)
                .ToListAsync();

            return applications;
        }

        public async Task CreateUserApplicationAsync(string vacancyId)
        {
            var userGuid = _currentUser.UserGuid ?? throw new Exception(MessageHelper.GetMessage("NOT_FOUND"));
            var vacancyGuid = Guid.Parse(vacancyId);
            var existApplication = await _context.Applications.Where(a => a.VacancyId == vacancyGuid && a.UserId == userGuid).FirstOrDefaultAsync();
            if (existApplication != null) throw new ApplicationIsAlreadyExistException(MessageHelper.GetMessage("APPLICATION_ALREADY_EXIST"));

            var vacancyInfo = await _context.Vacancies
                .Where(v => v.Id == vacancyGuid)
                .Select(v => new { v.CompanyId, v.Title })
                .FirstOrDefaultAsync() ?? throw new NotFoundException<Company>("NOT_FOUND");

            var status = await _context.Statuses.FirstOrDefaultAsync(s => s.Order == 1)
                ?? throw new NotFoundException<Status>(MessageHelper.GetMessage("NOT_FOUND"));

            var responseUser = await _requestUser.GetResponse<GetUserDataResponse>(
                new GetUserDataRequest { UserId = userGuid }
            );

            var newApplication = new Application
            {
                UserId = userGuid,
                VacancyId = vacancyGuid,
                StatusId = status.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            await _context.Applications.AddAsync(newApplication);
            await _context.SaveChangesAsync();


            await _publishEndpoint.Publish(
            new VacancyApplicationEvent
                {
                    UserId = vacancyInfo.CompanyId.Value,
                    SenderId = userGuid,
                    VacancyId = vacancyGuid,
                    InformationId = userGuid,
                Content = $"İstifadəçi {responseUser.Message.FirstName} {responseUser.Message.LastName} {vacancyInfo.Title} vakansiyasına müraciət etdi.",
            });
        }

        public async Task<PaginatedApplicationDto> GetUserApplicationsAsync(int skip, int take)
        {
            var userGuid = _currentUser.UserGuid;

            var query = _context.Applications
                .Where(a => a.UserId == userGuid);

            var totalCount = await query.CountAsync();

            var applications = await query
                .OrderByDescending(a => a.CreatedDate)
                .Skip(Math.Max(0, (skip - 1) * take))
                .Take(take)
                .Select(a => new ApplicationDto
                {
                    ApplicationId = a.Id,
                    VacancyId = a.VacancyId,
                    title = a.Vacancy.Title,
                    CompanyId = a.Vacancy.CompanyId,
                    companyLogo = a.Vacancy.Company != null ? a.Vacancy.Company.CompanyLogo : null,
                    CompanyName = a.Vacancy.Company != null ? a.Vacancy.Company.CompanyName : null,
                    WorkType = a.Vacancy.WorkType != null ? a.Vacancy.WorkType.ToString() : null, 
                    IsActive = a.IsActive,
                    StatusName = a.Status != null ? a.Status.StatusEnum.ToString() : null,
                    StatusColor = a.Status != null ? a.Status.StatusColor.ToString() : null,
                    ViewCount = a.Vacancy.ViewCount,
                    startDate = a.CreatedDate
                })
                .ToListAsync();

            return new PaginatedApplicationDto
            {
                Applications = applications,
                TotalCount = totalCount
            };
        }

        public async Task<GetApplicationDetailResponse> GetUserApplicationByIdAsync(string applicationId)
        {
            var userGuid = _currentUser.UserGuid;
            var applicationGuid = Guid.Parse(applicationId);

    
            var application = await _context.Applications
                .Include(a => a.Status.Translations)
                .Include(a => a.Vacancy)           
                .ThenInclude(v => v.Company)  
                .ThenInclude(c => c.Statuses)
                .ThenInclude(c => c.Translations)
                .Where(a => a.UserId == userGuid && a.Id == applicationGuid)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException<Application>(MessageHelper.GetMessage("NOT_FOUND"));

            return new GetApplicationDetailResponse
            {
                VacancyId = application.VacancyId.ToString(),
                VacancyName = application.Vacancy.Title,
                CompanyName = application.Vacancy.Company != null ? application.Vacancy.Company.CompanyName : string.Empty,
                CompanyLogo = application.Vacancy.Company != null ? application.Vacancy.Company.CompanyLogo : string.Empty,
                Location = application.Vacancy.Company != null ? application.Vacancy.Company.CompanyLocation : string.Empty,
                WorkType = application.Vacancy.WorkType,
                WorkStyle = application.Vacancy.WorkStyle,
                startDate = application.Vacancy.StartDate,
                //IsSaved = application.,
                CompanyStatuses = application.Vacancy.Company != null ? application.Vacancy.Company.Statuses.Select(cs => cs.GetTranslation(_currentUser.LanguageCode)).ToList() : new List<string>(),
                ApplicationStatus = application.Status.ToString()
            };
        }

    }
}