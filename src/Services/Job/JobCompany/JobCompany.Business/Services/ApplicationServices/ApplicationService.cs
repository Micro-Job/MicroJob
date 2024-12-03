using JobCompany.Business.Dtos.ApplicationDtos;
using JobCompany.Business.Dtos.StatusDtos;
using JobCompany.Business.Exceptions.ApplicationExceptions;
using JobCompany.Business.Exceptions.StatusExceptions;
using JobCompany.Business.Exceptions.VacancyExceptions;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Shared.Requests;
using Shared.Responses;
using SharedLibrary.Exceptions;
using System.Security.Claims;

namespace JobCompany.Business.Services.ApplicationServices
{
    public class ApplicationService : IApplicationService
    {
        private readonly JobCompanyDbContext _context;
        private readonly Guid userGuid;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _baseUrl;
        readonly IRequestClient<GetUsersDataResponse> _client;
        readonly IRequestClient<GetResumeDataResponse> _requestClient;
        readonly IPublishEndpoint _publishEndpoint;

        public ApplicationService(JobCompanyDbContext context, IRequestClient<GetUsersDataResponse> client, IRequestClient<GetResumeDataResponse> requestClient, IHttpContextAccessor contextAccessor, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _client = client;
            _requestClient = requestClient;
            _baseUrl = $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host.Value}{_contextAccessor.HttpContext.Request.PathBase.Value}";
            userGuid = Guid.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid).Value);
            _publishEndpoint = publishEndpoint;
        }

        /// <summary> Deyesen bu silinmeli idi əmin deyiləm </summary>
        public async Task CreateApplicationAsync(ApplicationCreateDto dto)
        {
            var VacancyId = Guid.Parse(dto.VacancyId);
            var vacancy = await _context.Vacancies
                .Where(v => v.Id == VacancyId)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException<Vacancy>();

            if (vacancy.IsActive == false) throw new VacancyStatusIsDeactiveException();

            var application = new Application
            {
                UserId = userGuid,
                VacancyId = vacancy.Id,
                IsActive = true
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
        }


        /// <summary> Yaradılan müraciətin geri alınması </summary>
        public async Task RemoveApplicationAsync(string applicationId)
        {
            var applicationGuid = Guid.Parse(applicationId);

            var existApplication = await _context.Applications.FirstOrDefaultAsync(x => x.Id == applicationGuid && x.UserId == userGuid)
            ?? throw new NotFoundException<Application>();
            if (existApplication.IsActive == false) throw new ApplicationStatusIsDeactiveException();
            existApplication.IsActive = false;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Müraciətin statusunun dəyişilməsi ve eventle usere bildiris publishi
        /// </summary>
        /// <param name="">Company name'ni where selectle aldim include elemedim </param>
        /// <returns></returns>
        public async Task ChangeApplicationStatusAsync(string applicationId, string statusId)
        {
            var statusGuid = Guid.Parse(statusId);
            var applicationGuid = Guid.Parse(applicationId);

            var existAppVacancy = await _context.Applications
                .Where(x => x.Id == applicationGuid && x.Vacancy.Company.UserId == userGuid)
                .Select(x => new
                {
                    Application = x,
                    CompanyName = x.Vacancy.Company.CompanyName
                })
                .FirstOrDefaultAsync() ?? throw new NotFoundException<Application>("Müraciət mövcud deyil!");

            var companyName = existAppVacancy.CompanyName;

            existAppVacancy.Application.StatusId = statusGuid;
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(new UpdateUserApplicationStatusEvent
            {
                UserId = existAppVacancy.Application.UserId,
                Content = $"{companyName} şirkətinin müraciət statusu dəyişdirildi: {existAppVacancy.Application.Status.StatusName}"
            });
        }



        /// <summary> Müraciətlərin statusu ilə birlikdə gətirilməsi </summary>
        public async Task<List<StatusListDtoWithApps>> GetAllApplicationWithStatusAsync(string vacancyId)
        {
            var vacancyGuid = Guid.Parse(vacancyId);

            var statuses = await _context.Statuses.Where(x => x.Company.UserId == userGuid || x.IsDefault == true).ToListAsync();

            var applications = await _context.Applications.Where(x => x.VacancyId == vacancyGuid && x.IsActive == true).ToListAsync();

            var groupedData = statuses.Select(status => new StatusListDtoWithApps
            {
                StatusId = status.Id,
                StatusName = status.StatusName,
                StatusColor = status.StatusColor,
                IsDefault = status.IsDefault,
                Applications = applications
            .Where(app => app.StatusId == status.Id)
                .Select(app => new ApplicationListDto
                {
                    ApplicationId = app.Id,
                    UserId = app.UserId,
                    VacancyId = app.VacancyId,
                    IsActive = app.IsActive
                }).Take(5).ToList()
            }).ToList();

            return groupedData;
        }

        /// <summary> vacancy ve statusa görə müraciətlərin gətirilməsi </summary>
        //bu metodun return-ü olmalıdır - tam deyil!!!!!!!!!!!!
        public async Task GetAllApplicationWithStatusAsync(string vacancyId, string statusId, int skip = 1, int take = 5)
        {
            var vacancyGuid = Guid.Parse(vacancyId);
            var statusGuid = Guid.Parse(statusId);

            var applications = await _context.Applications.Where(x => x.VacancyId == vacancyGuid && x.StatusId == statusGuid).Select(x => new
            {
                x.StatusId,
                x.UserId,
            })
            .Skip(Math.Max(0, (skip - 1) * take))
            .Take(take)
            .ToListAsync();
        }

        /// <summary> Userin müraciətlərinin gətirilməsi </summary>
        public async Task<List<ApplicationUserListDto>> GetUserApplicationAsync(int skip = 1, int take = 9)
        {
            var userApps = await _context.Applications.Where(x => x.UserId == userGuid).Select(x => new ApplicationUserListDto
            {
                CompanyName = x.Vacancy.Company.CompanyName,
                CreatedDate = x.CreatedDate,
                MainSalary = x.Vacancy.MainSalary,
                MaxSalary = x.Vacancy.MaxSalary,
                StatusColor = x.Status.StatusColor,
                StatusName = x.Status.StatusName,
                VacancyId = x.VacancyId,
                VacancyImage = $"{_baseUrl}/{x.Vacancy.CompanyLogo}",
                VacancyTitle = x.Vacancy.Title,
                ViewCount = x.Vacancy.ViewCount
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

            var application = await _context.Applications.Where(a => a.Id == applicationGuid && a.IsActive && a.UserId == userGuid)
            .Select(a => new ApplicationGetByIdDto
            {
                VacancyId = a.VacancyId,
                VacancyImage = a.Vacancy.CompanyLogo,
                VacancyTitle = a.Vacancy.Title,
                CompanyName = a.Vacancy.CompanyName,
                CreatedDate = a.CreatedDate,
                Description = a.Vacancy.Description,
                StatusName = a.Status.StatusName,
                StatusColor = a.Status.StatusColor,
                Steps = _context.Statuses
                    .OrderBy(s => s.Order)
                    .Select(s => s.StatusName)
                    .ToList()
            })
            .FirstOrDefaultAsync() ?? throw new NotFoundException<Application>();
            return application;
        }

        /// <summary> Consumer metodu - user idlərinə görə user datalarının gətirilməsi </summary>
        public async Task<GetUsersDataResponse> GetUserDataResponseAsync(List<Guid> userIds)
        {
            var request = new GetUsersDataRequest
            {
                UserIds = userIds
            };

            var response = await _client.GetResponse<GetUsersDataResponse>(request);

            var userDataResponse = new GetUsersDataResponse
            {
                Users = response.Message.Users
            };

            return userDataResponse;
        }


        /// <summary> Consumer metodu - Useridlere görə resumelerin getirilmesi </summary>
        public async Task<GetResumesDataResponse> GetResumeDataResponseAsync(List<Guid> userIds)
        {
            var request = new GetResumeDataRequest
            {
                UserIds = userIds
            };

            var response = await _requestClient.GetResponse<GetResumesDataResponse>(request);

            var userDataResponse = new GetResumesDataResponse
            {
                Users = response.Message.Users
            };

            return userDataResponse;
        }

        /// <summary> Şirkət üçün bütün müraciətlərin gətirilməsi </summary>
        public async Task<ICollection<ApplicationInfoListDto>> GetAllApplicationAsync(int skip = 1, int take = 9)
        {
            var applications = await _context.Applications
                .Include(a => a.Vacancy)
                .Where(a => a.Vacancy.CompanyId == userGuid)
                .Select(a => new ApplicationInfoDto
                {
                    UserId = a.UserId,
                    CreatedDate = a.CreatedDate
                })
                .ToListAsync();

            var userIds = applications.Select(a => a.UserId).ToList();

            var userDataResponse = await GetUserDataResponseAsync(userIds);
            var userResumeResponse = await GetResumeDataResponseAsync(userIds);

            var applicationList = applications
                .GroupBy(a => a.UserId)
                .SelectMany(group =>
                {
                    var userData = userDataResponse.Users.FirstOrDefault(u => u.UserId == group.Key);
                    var userResume = userResumeResponse.Users.FirstOrDefault(r => r.UserId == group.Key);

                    return group.Select(application => new ApplicationInfoListDto
                    {
                        FullName = $"{userData?.FirstName} {userData?.LastName}",
                        ImageUrl = userData?.ProfileImage,
                        Position = userResume?.Position,
                        CreatedDate = application.CreatedDate
                    });
                })
                .Skip((skip - 1) * take)
                .Take(take)
                .ToList();

            return applicationList;
        }


    }
}

