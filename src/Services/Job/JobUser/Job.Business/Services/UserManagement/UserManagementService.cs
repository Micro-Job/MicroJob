using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.Common;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Dtos.ResumeDtos;
using Job.Business.Dtos.SkillDtos;
using Job.Business.Dtos.UserDtos;
using Job.Business.Exceptions.Common;
using Job.Business.Extensions;
using Job.Business.Statistics;
using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Services.UserManagement;

public class UserManagementService(JobDbContext _context, IRequestClient<GetUserDataRequest> _userDataRequest, 
    IConfiguration _configuration, ICurrentUser _currentUser) : IUserManagementService
{
    public async Task<UserPersonalInfoDto> GetPersonalInfoAsync(string userId)
    {
        var userGuid = Guid.Parse(userId);
        var data = await _userDataRequest.GetResponse<GetUserDataResponse>(new GetUserDataRequest
        {
            UserId = userGuid
        });

        var resumeData = await _context.Resumes
            .Where(x => x.UserId == userGuid)
            .Select(x => new
            {
                x.BirthDay,
                x.IsDriver,
                x.IsMarried,
                x.MilitarySituation,
                PositionName = x.Position != null ? x.Position.Name : string.Empty
            }).FirstOrDefaultAsync();

        var personalInfo = new UserPersonalInfoDto
        {
            UserId = data.Message.UserId,
            FullName = $"{data.Message.FirstName} {data.Message.LastName}",
            Email = data.Message.Email,
            ImageUrl = $"{_configuration["AuthService:BaseUrl"]}{data.Message.ProfileImage}",
            MainPhoneNumber = data.Message.MainPhoneNumber,
        };

        if (resumeData != null)
        {
            personalInfo.BirthDay = resumeData.BirthDay;
            personalInfo.Position = resumeData.PositionName;
            personalInfo.MilitarySituation = resumeData.MilitarySituation;
            personalInfo.IsDriver = resumeData.IsDriver;
            personalInfo.FamilySituation = resumeData.IsMarried;
        }

        return personalInfo;
    }

    public async Task<ResumeDetailItemDto> GetResumeDetailAsync(string userId)
    {
        var userGuid = Guid.Parse(userId);

        var resume = await _context.Resumes.Where(r => r.UserId == userGuid)
        .Include(x => x.ResumeSkills)
            .ThenInclude(x => x.Skill)
                .ThenInclude(x => x.Translations)
        .Select(r => new ResumeDetailItemDto
        {
            UserId = r.UserId,
            ResumeId = r.Id,
            FirstName = r.FirstName,
            LastName = r.LastName,
            FatherName = r.FatherName,
            IsDriver = r.IsDriver,
            IsMarried = r.IsMarried,
            IsCitizen = r.IsCitizen,
            MilitarySituation = r.MilitarySituation,
            Gender = r.Gender,
            Adress = r.Adress,
            Position = r.Position != null ? r.Position.Name : null,
            BirthDay = r.BirthDay,
            ResumeEmail = r.ResumeEmail,
            UserPhoto = r.UserPhoto != null ? $"{_currentUser.BaseUrl}/{r.UserPhoto}" : null,
            Skills = r.ResumeSkills.Select(s => new SkillGetByIdDto
            {
                Id = s.SkillId,
                Name = s.Skill.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name)
            }).ToList(),
            PhoneNumbers = r.IsPublic ?
            r.PhoneNumbers.Select(p => new NumberGetByIdDto
            {
                PhoneNumber = p.PhoneNumber
            }).ToList()
            : null,
            Educations = r.Educations.Select(e => new EducationGetByIdDto
            {
                EducationId = e.Id,
                InstitutionName = e.InstitutionName,
                Profession = e.Profession,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                IsCurrentEducation = e.IsCurrentEducation,
                ProfessionDegree = e.ProfessionDegree
            }).ToList(),
            Experiences = r.Experiences.Select(ex => new ExperienceGetByIdDto
            {
                ExperienceId = ex.Id,
                OrganizationName = ex.OrganizationName,
                PositionName = ex.PositionName,
                PositionDescription = ex.PositionDescription,
                StartDate = ex.StartDate,
                EndDate = ex.EndDate,
                IsCurrentOrganization = ex.IsCurrentOrganization
            }).ToList(),
            Languages = r.Languages.Select(l => new LanguageGetByIdDto
            {
                LanguageId = l.Id,
                LanguageName = l.LanguageName,
                LanguageLevel = l.LanguageLevel
            }).ToList(),
            Certificates = r.Certificates.Select(c => new CertificateGetByIdDto
            {
                CertificateId = c.Id,
                CertificateName = c.CertificateName,
                GivenOrganization = c.GivenOrganization,
                CertificateFile = $"{_currentUser.BaseUrl}/{c.CertificateFile}"
            }).ToList()
        })
        .FirstOrDefaultAsync() ?? throw new NotFoundException<Core.Entities.Resume>();

        return resume;
    }
}
