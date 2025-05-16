using JobCompany.Business.Dtos.ApplicationDtos;
using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.StatusDtos;
using JobCompany.Business.Exceptions.ApplicationExceptions;
using JobCompany.Business.Exceptions.VacancyExceptions;
using JobCompany.Business.Extensions;
using JobCompany.Business.Statistics;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Extensions;
using Shared.Events;
using Shared.Requests;
using Shared.Responses;
using SharedLibrary.Dtos.ApplicationDtos;
using SharedLibrary.Enums;
using SharedLibrary.Events;
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
        private readonly IConfiguration _configuration;
        readonly IRequestClient<GetUsersDataRequest> _getUserDataClient;
        readonly IRequestClient<GetResumeDataRequest> _getResumeDataClient;
        readonly IPublishEndpoint _publishEndpoint;
        private readonly ICurrentUser _currentUser;
        private readonly string? _authServiceBaseUrl;
        private readonly IRequestClient<GetResumeIdsByUserIdsRequest> _resumeIdsRequest;
        private readonly IRequestClient<GetFilteredUserIdsRequest> _filteredUserIdsRequest;
        private readonly IRequestClient<GetUserResumePhotoRequest> _userPhotoRequest;


        public ApplicationService(
            JobCompanyDbContext context,
            IRequestClient<GetUsersDataRequest> client,
            IRequestClient<GetResumeDataRequest> requestClient,
            IPublishEndpoint publishEndpoint,
            ICurrentUser currentUser,
            IRequestClient<GetUserDataRequest> requestUser,
            IRequestClient<GetResumeIdsByUserIdsRequest> resumeIdsRequest,
            IRequestClient<GetFilteredUserIdsRequest> filteredUserIdsRequest,
            IRequestClient<GetUserResumePhotoRequest> userPhotoRequest,
            IConfiguration configuration)
        {
            _currentUser = currentUser;
            _context = context;
            _getUserDataClient = client;
            _getResumeDataClient = requestClient;
            _publishEndpoint = publishEndpoint;
            _resumeIdsRequest = resumeIdsRequest;
            _filteredUserIdsRequest = filteredUserIdsRequest;
            _userPhotoRequest = userPhotoRequest;
            _configuration = configuration;
            _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
        }

        /// <summary> Yaradılan müraciətin geri alınması </summary>
        public async Task RemoveApplicationAsync(string applicationId)
        {
            var applicationGuid = Guid.Parse(applicationId);

            var existApplication =
                await _context.Applications.FirstOrDefaultAsync(x =>
                    x.Id == applicationGuid && x.UserId == _currentUser.UserGuid)
                ?? throw new NotFoundException<Application>();
            if (existApplication.IsActive == false)
                throw new ApplicationStatusIsDeactiveException();
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
                .Where(x => x.Id == applicationGuid && x.Vacancy.Company.UserId == _currentUser.UserGuid)
                .Select(x => new
                {
                    Application = x,
                    VacancyId = x.VacancyId,
                    CompanyName = x.Vacancy.Company.CompanyName,
                    CompanyLogo = x.Vacancy.Company.CompanyLogo,
                    VacancyTitle = x.Vacancy.Title
                })
                .FirstOrDefaultAsync() ?? throw new NotFoundException<Application>();

            var application = existAppVacancy.Application;

            application.StatusId = statusGuid;
            await _context.SaveChangesAsync();

            //Müraciət statusu dəyişildikdə notification göndərilir
            await _publishEndpoint.Publish(
                new NotificationToUserEvent
                {
                    ReceiverIds = [application.UserId],
                    SenderId = (Guid)_currentUser.UserGuid,
                    InformationId = applicationGuid,
                    InformationName = existAppVacancy.VacancyTitle,
                    NotificationType = NotificationType.ApplicationStatusUpdate,
                    SenderImage = $"{_currentUser.BaseUrl}/{existAppVacancy.CompanyLogo}",
                    SenderName = existAppVacancy.CompanyName,
                }
            );
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

                        Status = a.Status.StatusEnum,
                        StatusColor = a.Status.StatusColor,
                        Steps = _context
                            .Statuses.OrderBy(s => s.Order)
                            .Select(s => s.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name))
                            .ToList(),
                    })
                    .FirstOrDefaultAsync() ?? throw new NotFoundException<Application>();

            return application;
        }

        /// <summary> Consumer metodu - user idlərinə görə user datalarının gətirilməsi </summary>
        private async Task<GetUsersDataResponse> GetUserDataResponseAsync(List<Guid> userIds, string? fullName)
        {
            var request = new GetUsersDataRequest { UserIds = userIds, FullName = fullName };

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
        public async Task<ICollection<ApplicationInfoListDto>> GetAllApplicationAsync(int skip = 1, int take = 9)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.UserId == _currentUser.UserGuid)
                ?? throw new NotFoundException<Company>();

            var applications = await _context.Applications
                .Where(a => a.Vacancy.Company.UserId == _currentUser.UserGuid)
                .Select(a => new ApplicationInfoDto
                {
                    ApplicationId = a.Id,
                    UserId = a.UserId,
                    CreatedDate = a.CreatedDate,
                })
                .ToListAsync();

            var userIds = applications.Select(a => a.UserId).ToList();

            var userResumeResponse = await GetResumeDataResponseAsync(userIds);

            var applicationList = applications
                .GroupBy(a => a.UserId)
                .SelectMany(group =>
                {
                    var userResume = userResumeResponse.Users.FirstOrDefault(r =>
                        r.UserId == group.Key
                    );

                    return group.Select(application => new ApplicationInfoListDto
                    {
                        ApplicationId = application.ApplicationId,
                        FullName = $"{userResume?.FirstName} {userResume?.LastName}",
                        ImageUrl = $"{_authServiceBaseUrl}/{userResume?.ProfileImage}",
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
        public async Task<DataListDto<AllApplicationListDto>> GetAllApplicationsListAsync(Guid? vacancyId, Gender? gender, StatusEnum? status, List<Guid>? skillIds, string? fullName, int skip = 1, int take = 10)
        {
            var applications = await GetPaginatedApplicationsAsync(skip, take, vacancyId, status);

            var userIds = applications.Item1.Select(a => a.UserId).ToList();

            var filteredApplications = applications.Item1;

            if (gender != null || (skillIds != null && skillIds.Count != 0)) //Filterdə gender və ya skillids varsa sorğu atılır
            {
                var response = await _filteredUserIdsRequest.GetResponse<GetFilteredUserIdsResponse>(
                    new GetFilteredUserIdsRequest { UserIds = userIds, Gender = gender, SkillIds = skillIds }); //Parametrlərə uyğun user id-ləri filtrlənir

                filteredApplications = applications.Item1.Where(a => response.Message.UserIds.Contains(a.UserId)).ToList();
            }

            var userDataResponse = await GetUserDataResponseAsync(filteredApplications.Select(a => a.UserId).ToList(), fullName); //Fullname userləri fullname-ə görə filterləmək üçündür
            var resumeIdsResponse = await GetResumeIdsByUserIds(filteredApplications.Select(a => a.UserId).ToList());

            var data = MapApplicationsToDto(filteredApplications, userDataResponse, resumeIdsResponse);

            return new DataListDto<AllApplicationListDto>
            {
                Datas = data,
                TotalCount = filteredApplications.Count
            };
        }

        private List<AllApplicationListDto> MapApplicationsToDto(List<Application> applications, GetUsersDataResponse usersDataResponse, GetResumeIdsByUserIdsResponse resumeIdsByUserIds)
        {
            var response = applications
                .Select(a =>
                {
                    var user = usersDataResponse.Users.FirstOrDefault(u => u.UserId == a.UserId);

                    var resumeId = resumeIdsByUserIds.ResumeIds.TryGetValue(a.UserId, out var id) ? id : Guid.Empty;

                    return new AllApplicationListDto()
                    {
                        ApplicationId = a.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        StatusId = a.StatusId,
                        Status = a.Status.StatusEnum,
                        VacancyId = a.VacancyId,
                        VacancyName = a.Vacancy.Title,
                        ProfileImage = $"{_authServiceBaseUrl}/{user.ProfileImage}",
                        DateTime = a.CreatedDate,
                        ResumeId = resumeId,
                    };
                })
                .ToList();

            return response;
        }

        private async Task<(List<Application>, int)> GetPaginatedApplicationsAsync(int skip, int take, Guid? vacancyId, StatusEnum? status)
        {
            var query = _context.Applications
                .Where(a => a.Vacancy.Company.UserId == _currentUser.UserGuid);

            if (vacancyId != null)  // Vakansiyaya görə filterlənmə
                query = query.Where(a => a.VacancyId == vacancyId);

            if (status != null) // Statusa görə filterlənmə
                query = query.Where(a => a.Status.StatusEnum == status);

            query = query.Include(a => a.Vacancy)
                         .Include(a => a.Status)
                         .OrderByDescending(a => a.CreatedDate);

            var applications = await query
                .Skip(Math.Max(0, (skip - 1) * take))
                .Take(take)
                .ToListAsync();

            return (applications, await query.CountAsync());
        }

        public async Task CreateUserApplicationAsync(string vacancyId)
        {
            var userGuid = _currentUser.UserGuid ?? throw new Exception(MessageHelper.GetMessage("NOT_FOUND"));

            var vacancyGuid = Guid.Parse(vacancyId);

            if (await _context.Applications.AnyAsync(x => x.VacancyId == vacancyGuid && x.IsActive == true && x.UserId == userGuid))
                throw new ApplicationIsAlreadyExistException();

            var vacancyInfo = await _context.Vacancies
                .Where(v => v.Id == vacancyGuid && v.VacancyStatus == VacancyStatus.Active && v.EndDate > DateTime.Now)
                .Select(v => new { v.Title, v.VacancyStatus, v.CompanyId, v.EndDate })
                .FirstOrDefaultAsync() ?? throw new NotFoundException<Company>();

            if (vacancyInfo.EndDate < DateTime.Now) throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));
            if (vacancyInfo.VacancyStatus == VacancyStatus.Pause) throw new VacancyPausedException();

            var companyStatus = await _context.Statuses.FirstOrDefaultAsync(x => x.StatusEnum == StatusEnum.Pending && x.CompanyId == vacancyInfo.CompanyId) ??
                throw new NotFoundException<StatusEnum>();

            var newApplication = new Application
            {
                UserId = userGuid,
                VacancyId = vacancyGuid,
                StatusId = companyStatus.Id,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            await _context.Applications.AddAsync(newApplication);

            var userPhotoResp = await _userPhotoRequest.GetResponse<GetUserResumePhotoResponse>(new GetUserResumePhotoRequest
            {
                UserId = userGuid
            });

            var notification = new Notification
            {
                SenderId = userGuid,
                SenderName = _currentUser.UserFullName,
                SenderImage = $"{_configuration["JobUser:BaseUrl"]}{userPhotoResp.Message.ImageUrl}",
                NotificationType = NotificationType.Application,
                CreatedDate = DateTime.Now,
                InformationId = userPhotoResp.Message.ResumeId,
                InformationName = vacancyInfo.Title,
                IsSeen = false,
                ReceiverId = (Guid)vacancyInfo.CompanyId!,
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedApplicationDto> GetUserApplicationsAsync(string? vacancyName, int skip, int take)
        {
            var query = _context.Applications
                .Where(a => a.UserId == _currentUser.UserGuid && a.IsActive);

            if (!string.IsNullOrEmpty(vacancyName)) // Vakansiya adına görə filterlənmə
            {
                vacancyName = vacancyName.Trim();
                query = query.Where(a => a.Vacancy.Title.Contains(vacancyName));
            }

            int totalCount = await query.CountAsync();

            var applications = await query
                .Include(x => x.Status)
                .OrderByDescending(a => a.CreatedDate)
                .Select(a => new ApplicationDto
                {
                    ApplicationId = a.Id,
                    VacancyId = a.VacancyId,
                    Title = a.Vacancy.Title,
                    CompanyId = a.Vacancy.CompanyId,
                    CompanyLogo = a.Vacancy.Company.CompanyLogo != null ? $"{_currentUser.BaseUrl}/{a.Vacancy.Company.CompanyLogo}" : null,
                    CompanyName = a.Vacancy.Company.CompanyName,
                    WorkType = a.Vacancy.WorkType,
                    VacancyStatus = a.Vacancy.VacancyStatus,
                    Status = a.Status.StatusEnum,
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
                .Include(x => x.Status)
                .Where(a => a.UserId == _currentUser.UserGuid && a.Id == applicationGuid)
                .Select(application => new ApplicationDetailDto
                {
                    VacancyId = application.VacancyId,
                    VacancyName = application.Vacancy.Title,
                    IsSavedVacancy = application.Vacancy.SavedVacancies.Any(x => x.UserId == _currentUser.UserGuid),
                    CompanyId = application.Vacancy.CompanyId,
                    CompanyName = application.Vacancy.Company.CompanyName,
                    CompanyLogo = $"{_currentUser.BaseUrl}/{application.Vacancy.Company.CompanyLogo}",
                    Location = application.Vacancy.Company.CompanyLocation,
                    WorkType = application.Vacancy.WorkType,
                    WorkStyle = application.Vacancy.WorkStyle,
                    CreatedDate = application.CreatedDate,
                    ApplicationStatusId = application.StatusId,
                    CompanyStatuses = application.Vacancy.Company.Statuses.Where(x => x.IsVisible).Select(x => new ApplicationStatusesListDto
                    {
                        CompanyStatusId = x.Id,
                        CompanyStatus = x.StatusEnum,
                        Order = x.Order
                    })
                    .OrderBy(x => x.Order)
                    .ToList()
                })
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException<Application>();

            return application;
        }

        /// <summary>
        /// Sadece 1 consumer responsundan istifade etdim , userDataResponsdaki datalar onsuzda resumeDataResponse'da var idi o birine ehtiyac qalmadi
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception> 
        public async Task<DataListDto<ApplicationWithStatusInfoListDto>> GetAllApplicationWithStatusAsync(int skip = 1, int take = 9)
        {
            var applications = await GetPaginatedApplicationsAsync(skip, take, null, null);

            var userIds = applications.Item1.Select(a => a.UserId).ToList();

            var resumeDataResponse = await GetResumeDataResponseAsync(userIds);
            var data = MapApplicationsWithStatusToDto(applications.Item1, resumeDataResponse);

            return new DataListDto<ApplicationWithStatusInfoListDto>
            {
                Datas = data,
                TotalCount = applications.Item2
            };
        }

        private List<ApplicationWithStatusInfoListDto> MapApplicationsWithStatusToDto(
            List<Application> applications,
            GetResumesDataResponse resumesDataResponse)
        {
            var response = applications
                .Select(a =>
                {
                    var resume = resumesDataResponse.Users.FirstOrDefault(r => r.UserId == a.UserId);

                    return new ApplicationWithStatusInfoListDto()
                    {
                        ApplicationId = a.Id,
                        ProfileImage = resume != null && resume.ProfileImage != null
                        ? $"{_authServiceBaseUrl}/{resume.ProfileImage}"
                        : null,

                        FirstName = resume?.FirstName,
                        LastName = resume?.LastName,
                        StatusId = a.StatusId,
                        StatusName = a.Status.StatusEnum,
                        Position = resume?.Position,
                        DateTime = a.CreatedDate,
                    };
                })
                .ToList();

            return response;
        }

        /// <summary> User id-lərinə görə resume id-lərinin gətirilməsi </summary>
        private async Task<GetResumeIdsByUserIdsResponse> GetResumeIdsByUserIds(List<Guid> userIds)
        {
            var request = new GetResumeIdsByUserIdsRequest { UserIds = userIds };
            var response = await _resumeIdsRequest.GetResponse<GetResumeIdsByUserIdsResponse>(request);
            return response.Message;
        }
    }
}