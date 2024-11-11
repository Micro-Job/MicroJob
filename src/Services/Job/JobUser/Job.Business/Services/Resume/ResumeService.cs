using Job.Business.Dtos.FileDtos;
using Job.Business.Dtos.ResumeDtos;
using Job.Business.Exceptions.Common;
using Job.Business.Exceptions.UserExceptions;
using Job.Business.ExternalServices;
using Job.Business.Services.Certificate;
using Job.Business.Services.Education;
using Job.Business.Services.Experience;
using Job.Business.Services.Language;
using Job.Business.Services.Number;
using Job.Business.Statics;
using Job.DAL.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Job.Business.Services.Resume
{
    public class ResumeService : IResumeService
    {
        readonly JobDbContext _context;
        readonly IFileService _fileService;
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly Guid _userId;
        readonly INumberService _numberService;
        readonly IEducationService _educationService;
        readonly IExperienceService _experienceService;
        readonly ILanguageService _languageService;
        readonly ICertificateService _certificateService;

        public ResumeService(JobDbContext context, IFileService fileService, IHttpContextAccessor httpContextAccessor, INumberService numberService,
            IEducationService educationService, IExperienceService experienceService, ILanguageService languageService, ICertificateService certificateService)
        {
            _context = context;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;
            _numberService = numberService;
            _educationService = educationService;
            _experienceService = experienceService;
            _languageService = languageService;
            _certificateService = certificateService;

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userId == null) throw new UserIsNotLoggedInException();
            _userId = Guid.Parse(userId.Value);
        }

        public async Task CreateResumeAsync(ResumeCreateDto resumeCreateDto)
        {
            FileDto fileResult = new();
            var resumeGuid = Guid.NewGuid();

            if (resumeCreateDto.UserPhoto != null)
                fileResult = await _fileService.UploadAsync(FilePaths.image, resumeCreateDto.UserPhoto);

            if (!resumeCreateDto.IsMainNumber)
            {
                var numbers = await _numberService.CreateBulkNumberAsync(resumeCreateDto.PhoneNumbers);
            }
            var educations = await _educationService.CreateBulkEducationAsync(resumeCreateDto.Educations);

            var experiences = await _experienceService.CreateBulkExperienceAsync(resumeCreateDto.Experiences);

            var languages = await _languageService.CreateBulkLanguageAsync(resumeCreateDto.Languages);

            var certificates = await _certificateService.CreateBulkCertificateAsync(resumeCreateDto.Certificates);

            var resume = new Core.Entities.Resume
            {
                Id = resumeGuid,
                UserId = _userId,
                FatherName = resumeCreateDto.FatherName,
                Position = resumeCreateDto.Position,
                IsDriver = resumeCreateDto.IsDriver,
                IsMarried = resumeCreateDto.IsMarried,
                IsCitizen = resumeCreateDto.IsCitizen,
                Gender = resumeCreateDto.Gender,
                Adress = resumeCreateDto.Adress,
                BirthDay = resumeCreateDto.BirthDay,
                UserPhoto = resumeCreateDto.UserPhoto != null
                    ? $"{fileResult.FilePath}/{fileResult.FileName}"
                    : null,
                Educations = educations,
                //PhoneNumbers = numbers,
                Experiences = experiences,
                Languages = languages,
                Certificates = certificates,
                //ResumeEmail = resumeCreateDto.IsMainEmail == false ? resumeCreateDto.ResumeEmail : existEmail,
            };
            await _context.Resumes.AddAsync(resume);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ResumeListDto>> GetAllResumeAsync()
        {
            var resumes = await _context.Resumes.ToListAsync();
            var resumeList = resumes.Select(r => new ResumeListDto
            {
                UserId = r.UserId,
                FatherName = r.FatherName,
                Position = r.Position,
                UserPhoto = r.UserPhoto,
                IsDriver = r.IsDriver,
                IsMarried = r.IsMarried,
                IsCitizen = r.IsCitizen,
                Gender = r.Gender,
                Adress = r.Adress,
                BirthDay = r.BirthDay
            });

            return resumeList;
        }

        public async Task<ResumeDetailItemDto> GetByIdResumeAsync(string id)
        {
            var resume = await _context.Resumes.FindAsync(Guid.Parse(id));
            if (resume is null) throw new NotFoundException<Core.Entities.Resume>();
            var resumeDetail = new ResumeDetailItemDto
            {
                UserId = resume.UserId,
                FatherName = resume.FatherName,
                Position = resume.Position,
                UserPhoto = resume.UserPhoto,
                IsDriver = resume.IsDriver,
                IsMarried = resume.IsMarried,
                IsCitizen = resume.IsCitizen,
                Gender = resume.Gender,
                Adress = resume.Adress,
                BirthDay = resume.BirthDay,
            };
            return resumeDetail;
        }

        public async Task UpdateResumeAsync(ResumeUpdateDto resumeUpdateDto)
        {
            var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.UserId == _userId)
                            ?? throw new NotFoundException<Core.Entities.Resume>();
            resume.FatherName = resumeUpdateDto.FatherName;
            resume.Position = resumeUpdateDto.Position;
            resume.IsDriver = resumeUpdateDto.IsDriver;
            resume.IsMarried = resumeUpdateDto.IsMarried;
            resume.IsCitizen = resumeUpdateDto.IsCitizen;
            resume.Gender = resumeUpdateDto.Gender;
            resume.Adress = resumeUpdateDto.Adress;
            resume.BirthDay = resumeUpdateDto.BirthDay;

            if (resumeUpdateDto.UserPhoto != null)
            {
                if (resume.UserPhoto != null)
                {
                    _fileService.DeleteFile(resume.UserPhoto);
                };
                var fileResult = await _fileService.UploadAsync(FilePaths.image, resumeUpdateDto.UserPhoto);
                resume.UserPhoto = $"{fileResult.FilePath}/{fileResult.FileName}";
            }

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
    }
}