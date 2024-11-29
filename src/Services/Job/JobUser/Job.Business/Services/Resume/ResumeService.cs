using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Dtos.ResumeDtos;
using Job.Business.Exceptions.Common;
using Job.Business.Services.Certificate;
using Job.Business.Services.Education;
using Job.Business.Services.Experience;
using Job.Business.Services.Language;
using Job.Business.Services.Number;
using Job.Business.Services.User;
using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Statics;
using System.Security.Claims;

namespace Job.Business.Services.Resume
{
    public class ResumeService : IResumeService
    {
        readonly JobDbContext _context;
        readonly IFileService _fileService;
        readonly INumberService _numberService;
        readonly IEducationService _educationService;
        readonly IExperienceService _experienceService;
        readonly ILanguageService _languageService;
        readonly ICertificateService _certificateService;
        readonly IUserInformationService _userInformationService;
        readonly IHttpContextAccessor _contextAccessor;
        private readonly Guid userGuid;
        private readonly string? _baseUrl;

        public ResumeService(JobDbContext context,
            IFileService fileService,
            INumberService numberService,
            IEducationService educationService,
            IExperienceService experienceService,
            ILanguageService languageService,
            ICertificateService certificateService,
            IUserInformationService userInformationService)
        {
            _context = context;
            _fileService = fileService;
            _numberService = numberService;
            _educationService = educationService;
            _experienceService = experienceService;
            _languageService = languageService;
            _certificateService = certificateService;
            _userInformationService = userInformationService;
            userGuid = Guid.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value);
            _baseUrl = $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host.Value}{_contextAccessor.HttpContext.Request.PathBase.Value}";
        }

        public async Task CreateResumeAsync(ResumeCreateDto resumeCreateDto, ResumeCreateListsDto resumeCreateListsDto)
        {
            if (await _context.Resumes.AnyAsync(x => x.UserId == userGuid))
                throw new IsAlreadyExistException<Core.Entities.Resume>();

            FileDto fileResult = resumeCreateDto.UserPhoto != null
                ? await _fileService.UploadAsync(FilePaths.document, resumeCreateDto.UserPhoto)
                : new FileDto();

            string email = resumeCreateDto.IsMainEmail
                ? await _userInformationService.GetUserDataAsync(userGuid).Select(x => x.Email)
                : resumeCreateDto.ResumeEmail;

            var resume = new Core.Entities.Resume
            {
                Id = Guid.NewGuid(),
                UserId = userGuid,
                FatherName = resumeCreateDto.FatherName,
                Position = resumeCreateDto.Position,
                IsDriver = resumeCreateDto.IsDriver,
                IsMarried = resumeCreateDto.IsMarried,
                IsCitizen = resumeCreateDto.IsCitizen,
                IsPublic = resumeCreateDto.IsPublic,
                Gender = resumeCreateDto.Gender,
                Adress = resumeCreateDto.Adress,
                BirthDay = resumeCreateDto.BirthDay,
                UserPhoto = resumeCreateDto.UserPhoto != null
                    ? $"{fileResult.FilePath}/{fileResult.FileName}"
                    : null,
                ResumeEmail = email
            };

            await _context.Resumes.AddAsync(resume);
            await _context.SaveChangesAsync();

            var numbers = new List<Core.Entities.Number>();
            if (!resumeCreateDto.IsMainNumber)
            {
                numbers = await _numberService.CreateBulkNumberAsync(resumeCreateListsDto.NumberCreateDtos.PhoneNumbers, resume.Id);
            }
            else
            {
                var mainNumber = await _userInformationService.GetUserDataAsync(userGuid).Select(x => new Core.Entities.Number
                {
                    PhoneNumber = x.MainPhoneNumber,
                    ResumeId = resume.Id
                });
                numbers.Add(mainNumber);
            }

            var educations = await _educationService.CreateBulkEducationAsync(resumeCreateListsDto.EducationCreateDtos.Educations, resume.Id);
            var experiences = await _experienceService.CreateBulkExperienceAsync(resumeCreateListsDto.ExperienceCreateDtos.Experiences, resume.Id);
            var languages = await _languageService.CreateBulkLanguageAsync(resumeCreateListsDto.LanguageCreateDtos.Languages, resume.Id);

            var certificates = resumeCreateDto.Certificates != null
                ? await _certificateService.CreateBulkCertificateAsync(resumeCreateDto.Certificates)
                : [];

            var resumeSkills = resumeCreateDto.SkillIds != null
                ? resumeCreateDto.SkillIds.Select(skillId => new ResumeSkill
                {
                    SkillId = skillId,
                    ResumeId = resume.Id
                }).ToList()
                : new();

            resume.PhoneNumbers = numbers;
            resume.Educations = educations;
            resume.Experiences = experiences;
            resume.Languages = languages;
            resume.Certificates = certificates;
            resume.ResumeSkills = resumeSkills;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateResumeAsync(ResumeUpdateDto resumeUpdateDto)
        {
            var resume = await _context.Resumes
                    .Include(r => r.PhoneNumbers)
                    .Include(r => r.Educations)
                    .Include(r => r.Experiences)
                    .Include(r => r.Languages)
                    .Include(r => r.Certificates).FirstOrDefaultAsync(r => r.UserId == userGuid)
                            ?? throw new NotFoundException<Core.Entities.Resume>();

            resume.FatherName = resumeUpdateDto.FatherName;
            resume.Position = resumeUpdateDto.Position;
            resume.IsDriver = resumeUpdateDto.IsDriver;
            resume.IsMarried = resumeUpdateDto.IsMarried;
            resume.IsCitizen = resumeUpdateDto.IsCitizen;
            resume.Gender = resumeUpdateDto.Gender;
            resume.Adress = resumeUpdateDto.Adress;
            resume.BirthDay = resumeUpdateDto.BirthDay;

            //if (resumeUpdateDto.UserPhoto != null)
            //{
            //    if (resume.UserPhoto != null)
            //    {
            //        _fileService.DeleteFile(resume.UserPhoto);
            //    };
            //    var fileResult = await _fileService.UploadAsync(FilePaths.image, resumeUpdateDto.UserPhoto);
            //    resume.UserPhoto = $"{fileResult.FilePath}/{fileResult.FileName}";
            //}

            if (resumeUpdateDto.PhoneNumbers != null)
            {
                foreach (var phoneNumberDto in resumeUpdateDto.PhoneNumbers)
                {
                    var phoneNumberGuid = Guid.Parse(phoneNumberDto.Id);
                    var phoneNumber = resume.PhoneNumbers.FirstOrDefault(p => p.Id == phoneNumberGuid);
                    if (phoneNumber != null && phoneNumber.PhoneNumber != phoneNumberDto.PhoneNumber)
                    {
                        phoneNumber.PhoneNumber = phoneNumberDto.PhoneNumber;
                    }
                }
            }

            if (resumeUpdateDto.Educations != null)
            {
                foreach (var educationDto in resumeUpdateDto.Educations)
                {
                    var educationGuid = Guid.Parse(educationDto.Id);
                    var education = resume.Educations.FirstOrDefault(e => e.Id == educationGuid);
                    if (education != null)
                    {
                        education.InstitutionName = educationDto.InstitutionName;
                        education.Profession = educationDto.Profession;
                        education.StartDate = educationDto.StartDate;
                        education.EndDate = educationDto.EndDate;
                    }
                }
            }

            if (resumeUpdateDto.Experiences != null)
            {
                foreach (var experienceDto in resumeUpdateDto.Experiences)
                {
                    var experienceGuid = Guid.Parse(experienceDto.Id);
                    var experience = resume.Experiences.FirstOrDefault(e => e.Id == experienceGuid);
                    if (experience != null)
                    {
                        experience.OrganizationName = experienceDto.OrganizationName;
                        experience.PositionName = experienceDto.PositionName;
                        experience.PositionDescription = experienceDto.PositionDescription;
                        experience.StartDate = experienceDto.StartDate;
                        experience.EndDate = experienceDto.EndDate;
                    }
                }
            }

            if (resumeUpdateDto.Languages != null)
            {
                foreach (var languageDto in resumeUpdateDto.Languages)
                {
                    var languageGuid = Guid.Parse(languageDto.Id);
                    var language = resume.Languages.FirstOrDefault(l => l.Id == languageGuid);
                    if (language != null && language.LanguageName != languageDto.LanguageName)
                    {
                        language.LanguageName = languageDto.LanguageName;
                    }
                }
            }

            if (resumeUpdateDto.Certificates != null)
            {
                foreach (var certificateDto in resumeUpdateDto.Certificates)
                {
                    var certificateGuid = Guid.Parse(certificateDto.Id);
                    var certificate = resume.Certificates.FirstOrDefault(c => c.Id == certificateGuid);
                    if (certificate != null && certificate.CertificateName != certificateDto.CertificateName)
                    {
                        certificate.CertificateName = certificateDto.CertificateName;
                        certificate.GivenOrganization = certificateDto.GivenOrganization;
                        if (certificate.CertificateFile != null)
                        {
                            _fileService.DeleteFile(certificate.CertificateFile);
                        }
                        var fileResult = await _fileService.UploadAsync(FilePaths.document, certificateDto.CertificateFile);
                        certificate.CertificateFile = $"{fileResult.FilePath}/{fileResult.FileName}";
                    }
                }
            }
            _context.Resumes.Update(resume);
            await _context.SaveChangesAsync();
        }

        public async Task<ResumeDetailItemDto> GetByIdResumeAsync()
        {
            var resume = await _context.Resumes
                                            .Include(x => x.PhoneNumbers)
                                            .Include(x => x.Educations)
                                            .Include(x => x.Certificates)
                                            .Include(x => x.Experiences)
                                            .Include(x => x.Languages)
                                            .FirstOrDefaultAsync(x => x.UserId == userGuid) ??
                                            throw new NotFoundException<Core.Entities.Resume>();

            var userFullName = await _userInformationService.GetUserDataAsync(userGuid).Select(x => new
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
            });

            var resumeDetail = new ResumeDetailItemDto
            {
                UserId = resume.UserId,
                FirstName = userFullName.FirstName,
                LastName = userFullName.LastName,
                FatherName = resume.FatherName,
                Position = resume.Position,
                UserPhoto = $"{_baseUrl}/{resume.UserPhoto}",
                IsDriver = resume.IsDriver,
                IsMarried = resume.IsMarried,
                IsCitizen = resume.IsCitizen,
                Gender = resume.Gender,
                Adress = resume.Adress,
                BirthDay = resume.BirthDay,
                ResumeEmail = resume.ResumeEmail,

                PhoneNumbers = resume.PhoneNumbers.Select(p => new NumberGetByIdDto
                {
                    PhoneNumber = p.PhoneNumber
                }).ToList(),

                Educations = resume.Educations.Select(e => new EducationGetByIdDto
                {
                    InstitutionName = e.InstitutionName,
                    Profession = e.Profession,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    IsCurrentEducation = e.IsCurrentEducation,
                    ProfessionDegree = e.ProfessionDegree
                }).ToList(),

                Experiences = resume.Experiences.Select(ex => new ExperienceGetByIdDto
                {
                    OrganizationName = ex.OrganizationName,
                    PositionName = ex.PositionName,
                    PositionDescription = ex.PositionDescription,
                    StartDate = ex.StartDate,
                    EndDate = ex.EndDate,
                    IsCurrentOrganization = ex.IsCurrentOrganization
                }).ToList(),

                Languages = resume.Languages.Select(l => new LanguageGetByIdDto
                {
                    LanguageName = l.LanguageName,
                    LanguageLevel = l.LanguageLevel
                }).ToList(),

                Certificates = resume.Certificates.Select(c => new CertificateGetByIdDto
                {
                    CertificateName = c.CertificateName,
                    GivenOrganization = c.GivenOrganization,
                    CertificateFile = c.CertificateFile
                }).ToList()
            };
            return resumeDetail;
        }
    }
}