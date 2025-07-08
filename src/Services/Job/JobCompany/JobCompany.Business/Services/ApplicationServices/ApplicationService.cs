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
using SharedLibrary.Dtos.ApplicationDtos;
using SharedLibrary.Enums;
using SharedLibrary.Events;
using SharedLibrary.Exceptions;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Services.ApplicationServices;

public class ApplicationService(JobCompanyDbContext _context, IPublishEndpoint _publishEndpoint, ICurrentUser _currentUser, IRequestClient<GetResumeDataRequest> _resumeDataRequest) 
{
    /// <summary> Vakansiya üçün müraciət yaradılması </summary>
    public async Task CreateUserApplicationAsync(Guid vacancyId)
    {
        var userGuid = _currentUser.UserGuid ?? throw new Exception(MessageHelper.GetMessage("NOT_FOUND"));

        if (await _context.Applications.AnyAsync(x => x.VacancyId == vacancyId && x.IsActive == true && x.UserId == userGuid))
            throw new ApplicationIsAlreadyExistException();

        var vacancyInfo = await _context.Vacancies
            .Where(v => v.Id == vacancyId && v.VacancyStatus == VacancyStatus.Active && v.EndDate > DateTime.Now)
            .Select(v => new { v.Title, v.VacancyStatus, v.CompanyId, v.EndDate })
            .FirstOrDefaultAsync() ?? throw new NotFoundException();

        if (vacancyInfo.EndDate < DateTime.Now) throw new BadRequestException();
        if (vacancyInfo.VacancyStatus == VacancyStatus.Pause) throw new VacancyPausedException();

        var companyStatus = await _context.Statuses.FirstOrDefaultAsync(x => x.StatusEnum == StatusEnum.Pending && x.CompanyId == vacancyInfo.CompanyId) ??
            throw new NotFoundException();

        var resumeData = await _resumeDataRequest.GetResponse<GetResumeDataResponse>(new GetResumeDataRequest { UserId = userGuid });

        var message = resumeData.Message;

        var newApplication = new Application
        {
            UserId = userGuid,
            VacancyId = vacancyId,
            StatusId = companyStatus.Id,
            IsActive = true,
            CreatedDate = DateTime.Now,
            ResumeId = message.ResumeId,
            ProfileImage = message.ProfileImage,
            Email = message.Email,
            FirstName = message.FirstName,
            LastName = message.LastName,
            PhoneNumber = message.PhoneNumber,
            Gender = message.Gender
        };

        await _context.Applications.AddAsync(newApplication);

        var notification = new Notification
        {
            SenderId = userGuid,
            SenderName = _currentUser.UserFullName,
            SenderImage = resumeData.Message.ProfileImage,
            NotificationType = NotificationType.Application,
            CreatedDate = DateTime.Now,
            InformationId = resumeData.Message.ResumeId,
            InformationName = vacancyInfo.Title,
            IsSeen = false,
            ReceiverId = (Guid)vacancyInfo.CompanyId!,
        };

        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    /// <summary> Yaradılan müraciətin geri alınması </summary>
    public async Task RemoveApplicationAsync(Guid applicationId)
    {
        var existApplication =
            await _context.Applications.FirstOrDefaultAsync(x =>
                x.Id == applicationId && x.UserId == _currentUser.UserGuid)
            ?? throw new NotFoundException();

        if (existApplication.IsActive == false)
            throw new ApplicationStatusIsDeactiveException();

        existApplication.IsActive = false;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Müraciətin statusunun dəyişilməsi ve eventle usere bildiris publishi
    /// </summary>
    public async Task ChangeApplicationStatusAsync(Guid applicationId, Guid statusId)
    {
        var existAppVacancy = await _context.Applications
            .Where(x => x.Id == applicationId && x.Vacancy.Company!.UserId == _currentUser.UserGuid)
            .Select(x => new
            {
                Application = x,
                x.VacancyId,
                x.Vacancy.Company!.CompanyName,
                x.Vacancy.Company.CompanyLogo,
                VacancyTitle = x.Vacancy.Title
            })
            .FirstOrDefaultAsync() ?? throw new NotFoundException();

        var application = existAppVacancy.Application;

        application.StatusId = statusId;
        await _context.SaveChangesAsync();

        //Müraciət statusu dəyişildikdə notification göndərilir
        await _publishEndpoint.Publish(
            new NotificationToUserEvent
            {
                ReceiverIds = [application.UserId],
                SenderId = (Guid)_currentUser.UserGuid!,
                InformationId = applicationId,
                InformationName = existAppVacancy.VacancyTitle,
                NotificationType = NotificationType.ApplicationStatusUpdate,
                SenderImage = existAppVacancy.CompanyLogo,
                SenderName = existAppVacancy.CompanyName,
            }
        );
    }

    /// <summary> Şirkətə daxil olan bütün müraciətlərin filterlə birlikdə detallı şəkildə gətirilməsi </summary>
    public async Task<DataListDto<AllApplicationListDto>> GetAllApplicationsListAsync(List<Guid>? vacancyIds, Gender? gender, List<StatusEnum>? status, string? fullName, StatusEnum? skipStatus, int skip = 1, int take = 10)
    {
        var query = GetApplicationsQuery(vacancyIds, status, fullName, skipStatus, gender);

        //TODO : bu hisse request responseden cixmalidir
        //if (skillIds != null && skillIds.Count != 0) //Filterdə gender və ya skillids varsa sorğu atılır
        //{
        //    var userIds = await query.Select(a => a.UserId).Distinct().ToListAsync();

        //    var response = await _filteredUserIdsRequest.GetResponse<GetFilteredUserIdsResponse>(
        //        new GetFilteredUserIdsRequest { UserIds = userIds, SkillIds = skillIds }); //Parametrlərə uyğun user id-ləri filtrlənir

        //    if (response.Message.UserIds.Count != 0)
        //        query = query.Where(a => response.Message.UserIds.Contains(a.UserId));
        //}

        var data = await query
            .OrderByDescending(a => a.CreatedDate)
            .Select(a => new AllApplicationListDto
            {
                ApplicationId = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email,
                ProfileImage = $"{_currentUser.BaseUrl}/userFiles/{a.ProfileImage}",
                DateTime = a.CreatedDate,
                ResumeId = a.ResumeId,
                PhoneNumber = a.PhoneNumber,
                StatusId = a.StatusId,
                Status = a.Status.StatusEnum,
                VacancyId = a.VacancyId,
                VacancyName = a.Vacancy.Title
            })
            .Skip((skip - 1) * take)
            .Take(take)
            .ToListAsync();

        return new DataListDto<AllApplicationListDto>
        {
            Datas = data,
            TotalCount = await query.CountAsync()
        };
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
                CompanyLogo = a.Vacancy.Company!.CompanyLogo != null ? $"{_currentUser.BaseUrl}/companyFiles/{a.Vacancy.Company.CompanyLogo}" : null,
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
            .Skip((skip - 1) * take)
            .Take(take)
            .ToListAsync();

        return new PaginatedApplicationDto
        {
            Applications = applications,
            TotalCount = totalCount
        };
    }

    public async Task<ApplicationDetailDto> GetUserApplicationByIdAsync(Guid applicationId)
    {
        var application = await _context.Applications
            .Include(x => x.Status)
            .Where(a => a.UserId == _currentUser.UserGuid && a.Id == applicationId)
            .Select(application => new ApplicationDetailDto
            {
                VacancyId = application.VacancyId,
                VacancyName = application.Vacancy.Title,
                IsSavedVacancy = application.Vacancy.SavedVacancies.Any(x => x.UserId == _currentUser.UserGuid),
                CompanyId = application.Vacancy.CompanyId,
                CompanyName = application.Vacancy.Company!.CompanyName,
                CompanyLogo = $"{_currentUser.BaseUrl}/companyFiles/{application.Vacancy.Company.CompanyLogo}",
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
            ?? throw new NotFoundException();

        return application;
    }

    private IQueryable<Application> GetApplicationsQuery(List<Guid>? vacancyIds, List<StatusEnum>? statuses, string? userFullName, StatusEnum? skipStatus, Gender? gender)
    {
        var query = _context.Applications.AsNoTracking()
            .Where(a => a.Vacancy.Company!.UserId == _currentUser.UserGuid && a.IsActive && !a.IsDeleted);

        if (vacancyIds != null && vacancyIds.Count > 0) // Vakansiyaya görə filterlənmə
            query = query.Where(a => vacancyIds.Contains(a.VacancyId));

        if (statuses != null && statuses.Count > 0) // Statusa görə filterlənmə
            query = query.Where(a => statuses.Contains(a.Status.StatusEnum));

        if (!string.IsNullOrEmpty(userFullName)) // Fullname-a görə filterlənmə
            query = query.Where(a => (a.FirstName + a.LastName).Contains(userFullName));

        if (skipStatus != null)
            query = query.Where(x => x.Status.StatusEnum == skipStatus);

        if (gender != null)
            query = query.Where(x=> x.Gender == gender);

        return query;
    }
}