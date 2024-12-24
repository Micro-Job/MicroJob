using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Dtos.ResumeDtos;
using Job.Business.Dtos.SkillDtos;
using Job.Business.Exceptions.Common;
using Job.Business.Exceptions.UserExceptions;
using Job.Business.Services.Certificate;
using Job.Business.Services.Education;
using Job.Business.Services.Experience;
using Job.Business.Services.Language;
using Job.Business.Services.Number;
using Job.Business.Services.User;
using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Requests;
using Shared.Responses;
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
        private readonly IRequestClient<GetResumeUserPhotoRequest> _resumeUser;
        readonly IConfiguration _configuration;
        private readonly string? _authServiceBaseUrl;

        public ResumeService(JobDbContext context,
            IFileService fileService,
            INumberService numberService,
            IEducationService educationService,
            IExperienceService experienceService,
            ILanguageService languageService,
            ICertificateService certificateService,
            IHttpContextAccessor httpContextAccess,
            IUserInformationService userInformationService,
            IRequestClient<GetResumeUserPhotoRequest> resumeUser,
            IConfiguration configuration)
        {
            _context = context;
            _fileService = fileService;
            _numberService = numberService;
            _educationService = educationService;
            _experienceService = experienceService;
            _languageService = languageService;
            _certificateService = certificateService;
            _userInformationService = userInformationService;
            _contextAccessor = httpContextAccess;
            userGuid = Guid.Parse(_contextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.Sid)?.Value ?? throw new UserIsNotLoggedInException());
            _baseUrl = $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host.Value}{_contextAccessor.HttpContext.Request.PathBase.Value}";
            _resumeUser = resumeUser;
            _configuration = configuration;
            _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
        }

        public async Task CreateResumeAsync(ResumeCreateDto resumeCreateDto, ResumeCreateListsDto resumeCreateListsDto)
        {
            if (await _context.Resumes.AnyAsync(x => x.UserId == userGuid))
                throw new IsAlreadyExistException<Core.Entities.Resume>();

            FileDto fileResult = resumeCreateDto.UserPhoto != null
                ? await _fileService.UploadAsync(FilePaths.document, resumeCreateDto.UserPhoto)
                : new FileDto();

            string? email = resumeCreateDto.IsMainEmail
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
                : [];

            resume.PhoneNumbers = numbers;
            resume.Educations = educations;
            resume.Experiences = experiences;
            resume.Languages = languages;
            resume.Certificates = certificates;
            resume.ResumeSkills = resumeSkills;

            await _context.SaveChangesAsync();
        }

        #region Before
        //public async Task UpdateResumeAsync(ResumeUpdateDto resumeUpdateDto, ResumeUpdateListDto resumeUpdateListsDto)
        //{
        //    var resume = await _context.Resumes
        //        .Include(x => x.PhoneNumbers)
        //        .Include(x => x.Educations)
        //        .Include(x => x.Certificates)
        //        .Include(x => x.Experiences)
        //        .Include(x => x.Languages)
        //        .FirstOrDefaultAsync(x => x.UserId == userGuid) ??
        //        throw new NotFoundException<Core.Entities.Resume>();

        //    resume.FatherName = resumeUpdateDto.FatherName;
        //    resume.Position = resumeUpdateDto.Position;
        //    resume.IsDriver = resumeUpdateDto.IsDriver;
        //    resume.IsMarried = resumeUpdateDto.IsMarried;
        //    resume.IsCitizen = resumeUpdateDto.IsCitizen;
        //    resume.IsPublic = resumeUpdateDto.IsPublic;
        //    resume.Gender = resumeUpdateDto.Gender;
        //    resume.Adress = resumeUpdateDto.Adress;
        //    resume.BirthDay = resumeUpdateDto.BirthDay;

        //    if (resumeUpdateDto.UserPhoto != null)
        //    {
        //        if (resume.UserPhoto != null) _fileService.DeleteFile(resume.UserPhoto);

        //        FileDto fileResult = await _fileService.UploadAsync(FilePaths.document, resumeUpdateDto.UserPhoto);
        //        resume.UserPhoto = $"{fileResult.FilePath}/{fileResult.FileName}";
        //    }

        //    string? email = resumeUpdateDto.IsMainEmail
        //        ? await _userInformationService.GetUserDataAsync(userGuid).Select(x => x.Email)
        //        : resumeUpdateDto.ResumeEmail;
        //    resume.ResumeEmail = email;

        //    if (!resumeUpdateDto.IsMainNumber)
        //    {
        //        _context.Numbers.RemoveRange(resume.PhoneNumbers);
        //        var numbers = await _numberService.UpdateBulkNumberAsync(resumeUpdateListsDto.NumberUpdateDtos.PhoneNumbers, resume.Id);
        //        resume.PhoneNumbers = numbers;
        //    }
        //    else
        //    {
        //        var mainNumber = await _userInformationService.GetUserDataAsync(userGuid).Select(x => new Core.Entities.Number
        //        {
        //            PhoneNumber = x.MainPhoneNumber,
        //            ResumeId = resume.Id
        //        });
        //        resume.PhoneNumbers = [mainNumber];
        //    }

        //    var educations = await _educationService.UpdateBulkEducationAsync(resumeUpdateListsDto.EducationUpdateDtos.Educations, resume.Id);
        //    resume.Educations = educations;

        //    var experiences = await _experienceService.UpdateBulkExperienceAsync(resumeUpdateListsDto.ExperienceUpdateDtos.Experiences, resume.Id);
        //    resume.Experiences = experiences;

        //    var languages = await _languageService.UpdateBulkLanguageAsync(resumeUpdateListsDto.LanguageUpdateDtos.Languages, resume.Id);
        //    resume.Languages = languages;

        //    var certificates = resumeUpdateDto.Certificates != null
        //        ? await _certificateService.UpdateBulkCertificateAsync(resumeUpdateDto.Certificates)
        //        : [];
        //    resume.Certificates = certificates;

        //    var resumeSkills = resumeUpdateDto.SkillIds != null
        //        ? resumeUpdateDto.SkillIds.Select(skillId => new ResumeSkill
        //        {
        //            SkillId = skillId,
        //            ResumeId = resume.Id
        //        }).ToList()
        //        : [];
        //    resume.ResumeSkills = resumeSkills;

        //    await _context.SaveChangesAsync();
        //} 
        #endregion

        public async Task UpdateResumeAsync(ResumeUpdateDto updateDto, ResumeUpdateListDto updateListsDto)
        {
            var resume = await GetResumeByUserIdAsync(userGuid);

            UpdateResumePersonalInfo(resume, updateDto);

            if (updateDto.UserPhoto != null) UpdateUserPhotoAsync(resume, updateDto.UserPhoto);

            await UpdateResumeEmailAsync(resume, updateDto);

            await UpdatePhoneNumbersAsync(resume, updateListsDto, updateDto.IsMainNumber);

            resume.Educations = await _educationService.UpdateBulkEducationAsync(updateListsDto.EducationUpdateDtos.Educations, resume.Id);
            resume.Experiences = await _experienceService.UpdateBulkExperienceAsync(updateListsDto.ExperienceUpdateDtos.Experiences, resume.Id);
            resume.Languages = await _languageService.UpdateBulkLanguageAsync(updateListsDto.LanguageUpdateDtos.Languages, resume.Id);
            resume.Certificates = await UpdateCertificatesAsync(updateDto.Certificates ?? []);

            UpdateResumeSkills(resume, updateDto.SkillIds);

            await _context.SaveChangesAsync();
        }

        private async Task<Core.Entities.Resume> GetResumeByUserIdAsync(Guid userGuid)
        {
            return await _context.Resumes
                .Include(r => r.PhoneNumbers)
                .Include(r => r.Educations)
                .Include(r => r.Certificates)
                .Include(r => r.Experiences)
                .Include(r => r.Languages)
                .FirstOrDefaultAsync(r => r.UserId == userGuid)
                ?? throw new NotFoundException<Core.Entities.Resume>();
        }

        private static void UpdateResumePersonalInfo(Core.Entities.Resume resume, ResumeUpdateDto updateDto)
        {
            resume.FatherName = updateDto.FatherName;
            resume.Position = updateDto.Position;
            resume.IsDriver = updateDto.IsDriver;
            resume.IsMarried = updateDto.IsMarried;
            resume.IsCitizen = updateDto.IsCitizen;
            resume.IsPublic = updateDto.IsPublic;
            resume.Gender = updateDto.Gender;
            resume.Adress = updateDto.Adress;
            resume.BirthDay = updateDto.BirthDay;
        }

        private async void UpdateUserPhotoAsync(Core.Entities.Resume resume, IFormFile userPhoto)
        {
            if (!string.IsNullOrEmpty(resume.UserPhoto))
            {
                _fileService.DeleteFile(resume.UserPhoto);
            }

            var fileResult = await _fileService.UploadAsync(FilePaths.document, userPhoto);
            resume.UserPhoto = $"{fileResult.FilePath}/{fileResult.FileName}";
        }

        private async Task UpdateResumeEmailAsync(Core.Entities.Resume resume, ResumeUpdateDto updateDto)
        {
            resume.ResumeEmail = updateDto.IsMainEmail
                ? (await _userInformationService.GetUserDataAsync(userGuid)).Email
                : updateDto.ResumeEmail;
        }

        private async Task UpdatePhoneNumbersAsync(Core.Entities.Resume resume, ResumeUpdateListDto updateListsDto, bool isMainNumber)
        {
            if (isMainNumber)
            {
                var mainNumber = new List<Core.Entities.Number>
                {
                    new() {
                        PhoneNumber = _userInformationService.GetUserDataAsync(userGuid).Result.MainPhoneNumber,
                        ResumeId = resume.Id
                    }
                };

                resume.PhoneNumbers = mainNumber;
            }
            else
            {
                _context.Numbers.RemoveRange(resume.PhoneNumbers);
                resume.PhoneNumbers = await _numberService.UpdateBulkNumberAsync(updateListsDto.NumberUpdateDtos.PhoneNumbers, resume.Id);
            }
        }

        private async Task<ICollection<Core.Entities.Certificate>> UpdateCertificatesAsync(ICollection<CertificateUpdateDto> certificatesDto)
        {
            return certificatesDto != null
                ? await _certificateService.UpdateBulkCertificateAsync(certificatesDto)
                : [];
        }

        private static void UpdateResumeSkills(Core.Entities.Resume resume, IEnumerable<Guid>? skillIds)
        {
            resume.ResumeSkills = skillIds?.Select(skillId => new ResumeSkill
            {
                SkillId = skillId,
                ResumeId = resume.Id
            }).ToList() ?? [];
        }

        public async Task<ResumeDetailItemDto> GetOwnResumeAsync()
        {
            var resume = await _context.Resumes
                                        .Where(x => x.UserId == userGuid)
                                        .Select(resume => new ResumeDetailItemDto
                                        {
                                            UserId = resume.UserId,
                                            FatherName = resume.FatherName,
                                            Position = resume.Position,
                                            IsDriver = resume.IsDriver,
                                            IsMarried = resume.IsMarried,
                                            IsCitizen = resume.IsCitizen,
                                            Gender = resume.Gender,
                                            Adress = resume.Adress,
                                            BirthDay = resume.BirthDay,
                                            ResumeEmail = resume.ResumeEmail,
                                            Skills = resume.ResumeSkills.Select(s => new SkillGetByIdDto
                                            {
                                                Skill = s.Skill.Name
                                            }).ToList(),
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
                                        })
                                        .FirstOrDefaultAsync()
                                        ?? throw new NotFoundException<Core.Entities.Resume>();

            var userFullName = await _userInformationService.GetUserDataAsync(userGuid);
            resume.FirstName = userFullName.FirstName;
            resume.LastName = userFullName.LastName;

            var response = await _resumeUser.GetResponse<GetResumeUserPhotoResponse>(new GetResumeUserPhotoRequest
            {
                UserId = userGuid
            });

            resume.UserPhoto = $"{_authServiceBaseUrl}/{response.Message.ProfileImage}";

            return resume;
        }
    }
}