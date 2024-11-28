﻿using AuthService.Business.Services.CurrentUser;
using JobCompany.Business.Dtos.ApplicationDtos;
using JobCompany.Business.Dtos.StatusDtos;
using JobCompany.Business.Exceptions.ApplicationExceptions;
using JobCompany.Business.Exceptions.StatusExceptions;
using JobCompany.Business.Exceptions.VacancyExceptions;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Requests;
using Shared.Responses;
using SharedLibrary.Exceptions;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Services.ApplicationServices
{
    public class ApplicationService : IApplicationService
    {
        private readonly JobCompanyDbContext _context;
        private readonly ICurrentUser _currentUser;
        private readonly Guid userGuid;
        readonly IRequestClient<GetUsersDataResponse> _client;
        readonly IRequestClient<GetResumeDataResponse> _requestClient;

        public ApplicationService(ICurrentUser currentUser, JobCompanyDbContext context, IRequestClient<GetUsersDataResponse> client, IRequestClient<GetResumeDataResponse> requestClient)
        {
            _context = context;
            _currentUser = currentUser;
            userGuid = Guid.Parse(_currentUser.UserId);
            _client = client;
            _requestClient = requestClient;
        }

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

        public async Task ChangeApplicationStatusAsync(string applicationId, string statusId)
        {
            var statusGuid = Guid.Parse(statusId);
            var applicationGuid = Guid.Parse(applicationId);

            var existAppVacancy = await _context.Applications
                                                .Include(x => x.Vacancy)
                                                    .ThenInclude(x => x.Company).FirstOrDefaultAsync(x => x.Id == applicationGuid)
                ?? throw new NotFoundException<Application>("Müraciət mövcud deyil!");

            if (existAppVacancy.Vacancy.Company.UserId != userGuid) throw new StatusPermissionException();

            existAppVacancy.StatusId = statusGuid;
            await _context.SaveChangesAsync();
        }

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

        //bu metodun return-ü olmalıdır
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
                VacancyImage = $"{_currentUser.BaseUrl}/{x.Vacancy.CompanyLogo}",
                VacancyTitle = x.Vacancy.Title,
                ViewCount = x.Vacancy.ViewCount
            })
            .Skip(Math.Max(0, (skip - 1) * take))
            .Take(take)
            .ToListAsync();

            return userApps;
        }

        public async Task<ApplicationGetByIdDto> GetApplicationByIdAsync(string applicationId)
        {
            var applicationGuid = Guid.Parse(applicationId);

            var application = await _context.Applications
        .Where(a => a.Id == applicationGuid && a.IsActive && a.UserId == userGuid)
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
                .Skip((skip - 1) * take)
                .Take(take)
                .ToListAsync();

            var userIds = applications.Select(a => a.UserId).ToList();

            var userDataResponse = await GetUserDataResponseAsync(userIds);
            var userResumeResponse = await GetResumeDataResponseAsync(userIds);

            var applicationList = new List<ApplicationInfoListDto>();

            foreach (var userId in userIds)
            {
                var userData = userDataResponse.Users.FirstOrDefault(u => u.UserId == userId);

                var userResume = userResumeResponse.Users.FirstOrDefault(r => r.UserId == userId);

                var userApplications = applications.Where(a => a.UserId == userId).ToList();

                foreach (var application in userApplications)
                {
                    applicationList.Add(new ApplicationInfoListDto
                    {
                        FullName = userData.FirstName + " " + userData.LastName,
                        ImageUrl = userData?.ProfileImage,
                        Position = userResume?.Position,
                        CreatedDate = application.CreatedDate
                    });
                }
            }

            return applicationList;
        }

    }
}