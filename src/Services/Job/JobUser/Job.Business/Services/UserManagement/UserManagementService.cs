using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.LinkDtos;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Dtos.ResumeDtos;
using Job.Business.Dtos.SkillDtos;
using Job.Business.Dtos.UserDtos;
using Job.Business.Extensions;
using Job.Business.Statistics;
using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Exceptions;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Services.UserManagement;

public class UserManagementService(JobDbContext _context, IConfiguration _configuration, ICurrentUser _currentUser) 
{
    public async Task<UserPersonalInfoDto> GetPersonalInfoAsync(Guid userId)
    {
        var personalInfo = await _context.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => new UserPersonalInfoDto
            {
                UserId = x.Id,
                BirthDay = x.Resume.BirthDay,
                IsDriver = x.Resume.IsDriver,
                FamilySituation = x.Resume.IsMarried,
                MilitarySituation = x.Resume.MilitarySituation,
                Position = x.Resume.Position != null ? x.Resume.Position.Name : string.Empty,
                FullName = $"{x.FirstName} {x.LastName}",
                Email = x.Email,
                MainPhoneNumber = x.MainPhoneNumber,
                ImageUrl = $"{_configuration["AuthService:BaseUrl"]}/userFiles/{x.Image}",
            }).FirstOrDefaultAsync() ?? throw new NotFoundException();

        return personalInfo;
    }

    public async Task<ResumeDetailItemDto> GetResumeDetailAsync(Guid userId)
    {
        var resume = await _context.Resumes
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .Include(x => x.ResumeSkills)
                .ThenInclude(x => x.Skill)
                    .ThenInclude(x => x.Translations)
            .Select(r => new ResumeDetailItemDto
            {
                UserId = r.UserId,
                ResumeId = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                IsDriver = r.IsDriver,
                IsMarried = r.IsMarried,
                IsCitizen = r.IsCitizen,
                MilitarySituation = r.MilitarySituation,
                Gender = r.Gender,
                Adress = r.Adress,
                Position = r.Position != null ? r.Position.Name : null,
                BirthDay = r.BirthDay,
                ResumeEmail = r.ResumeEmail,
                UserPhoto = r.UserPhoto != null ? $"{_currentUser.BaseUrl}/userFiles/{r.UserPhoto}" : null,
                Summary = r.Summary,
                Skills = r.ResumeSkills.Select(s => new SkillGetByIdDto
                {
                    Id = s.SkillId,
                    Name = s.Skill.Translations.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name)
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
                    CertificateFile = $"{_currentUser.BaseUrl}/userFiles/{c.CertificateFile}"
                }).ToList(),
                Urls = r.ResumeLinks.Select(x=> new LinkDto
                {
                    LinkType = x.LinkType,
                    Url = x.Url
                }).ToList()
            }).FirstOrDefaultAsync() ?? throw new NotFoundException();

        return resume;
    }
}
