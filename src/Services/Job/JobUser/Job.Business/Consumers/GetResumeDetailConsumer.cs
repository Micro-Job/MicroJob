using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Dtos.ResumeDtos;
using Job.Business.Dtos.SkillDtos;
using Job.Business.Exceptions.Common;
using Job.Business.Extensions;
using Job.Business.Statistics;
using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.ResumeDtos;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Consumers;

public class GetResumeDetailConsumer(JobDbContext _context, ICurrentUser _currentUser) : IConsumer<GetResumeDetailRequest>
{
    public async Task Consume(ConsumeContext<GetResumeDetailRequest> context)
    {
        var userId = _currentUser.UserGuid;
        //var resumeGuid = Guid.Parse(id);
        var resume = await _context.Resumes.Where(r => r.UserId == context.Message.UserId)
        .Include(x => x.ResumeSkills)
            .ThenInclude(x => x.Skill)
                .ThenInclude(x => x.Translations)
        .Include(x => x.SavedResumes)
        .AsNoTracking()
        .Select(r => new GetResumeDetailResponse
        {
            UserId = r.UserId,
            ResumeId = r.Id,
            IsSaved = r.SavedResumes.Any(sr => sr.CompanyUserId == userId),
            FirstName = r.FirstName ,
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
            UserPhoto = $"{_currentUser.BaseUrl}/{r.UserPhoto}",
            Skills = r.ResumeSkills.Select(s =>
                s.Skill.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name)
            ).ToList(),
            PhoneNumbers = r.PhoneNumbers.Select(p => p.PhoneNumber).ToList(),
            Educations = r.Educations.Select(e => new EducationDto
            {
                InstitutionName = e.InstitutionName,
                Profession = e.Profession,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                IsCurrentEducation = e.IsCurrentEducation,
                ProfessionDegree = (byte)e.ProfessionDegree
            }).ToList(),
            Experiences = r.Experiences.Select(ex => new ExperienceDto
            {
                OrganizationName = ex.OrganizationName,
                PositionName = ex.PositionName,
                PositionDescription = ex.PositionDescription,
                StartDate = ex.StartDate,
                EndDate = ex.EndDate,
                IsCurrentOrganization = ex.IsCurrentOrganization
            }).ToList(),
            Languages = r.Languages.Select(l => new LanguageDto
            {
                LanguageName = (byte)l.LanguageName,
                LanguageLevel = (byte)l.LanguageLevel
            }).ToList(),
            Certificates = r.Certificates.Select(c => new CertificateDto
            {
                CertificateName = c.CertificateName,
                GivenOrganization = c.GivenOrganization,
                CertificateFile = $"{_currentUser.BaseUrl}/{c.CertificateFile}"
            }).ToList()
        })
        .FirstOrDefaultAsync() ?? throw new NotFoundException<Core.Entities.Resume>(MessageHelper.GetMessage("NOT_FOUND"));

        await context.RespondAsync(resume);
    }
}
