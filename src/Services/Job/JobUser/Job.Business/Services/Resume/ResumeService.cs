using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Dtos.ResumeDtos;
using Job.Business.Dtos.SkillDtos;
using Job.Business.Exceptions.Common;
using Job.Business.Exceptions.UserExceptions;
using Job.Business.Extensions;
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
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Statics;
using System.Security.Claims;

namespace Job.Business.Services.Resume
{
    public class ResumeService : IResumeService
    {
        readonly JobDbContext _context;
        readonly IFileService _fileService;
        readonly ICurrentUser _currentUser;
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
            IConfiguration configuration,
            ICurrentUser currentUser)
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
            _currentUser = currentUser;
        }

        public async Task CreateResumeAsync(ResumeCreateDto resumeCreateDto, ResumeCreateListsDto resumeCreateListsDto)
        {
            if (await _context.Resumes.AnyAsync(x => x.UserId == userGuid))
                throw new IsAlreadyExistException<Core.Entities.Resume>();

            var resume = await BuildResumeAsync(resumeCreateDto);

            await _context.Resumes.AddAsync(resume);
            await _context.SaveChangesAsync();

            var phoneNumbers = await GetPhoneNumbersAsync(resumeCreateDto, resume.Id, resumeCreateListsDto);
            var educations = await _educationService.CreateBulkEducationAsync(resumeCreateListsDto.EducationCreateDtos.Educations, resume.Id);
            var experiences = await _experienceService.CreateBulkExperienceAsync(resumeCreateListsDto.ExperienceCreateDtos.Experiences, resume.Id);
            var languages = await _languageService.CreateBulkLanguageAsync(resumeCreateListsDto.LanguageCreateDtos.Languages, resume.Id);
            var certificates = await GetCertificatesAsync(resumeCreateDto);
            var resumeSkills = GetResumeSkills(resumeCreateDto.SkillIds, resume.Id);

            resume.PhoneNumbers = phoneNumbers;
            resume.Educations = educations;
            resume.Experiences = experiences;
            resume.Languages = languages;
            resume.Certificates = certificates;
            resume.ResumeSkills = resumeSkills;

            await _context.SaveChangesAsync();
        }

        private async Task<Core.Entities.Resume> BuildResumeAsync(ResumeCreateDto dto)
        {
            FileDto fileResult = dto.UserPhoto != null
                ? await _fileService.UploadAsync(FilePaths.document, dto.UserPhoto)
                : new FileDto();

            string? email = dto.IsMainEmail
                ? (await _userInformationService.GetUserDataAsync(userGuid)).Email
                : dto.ResumeEmail;

            return MapToResumeEntity(dto, $"{fileResult.FilePath}/{fileResult.FileName}", email);
        }

        private Core.Entities.Resume MapToResumeEntity(ResumeCreateDto dto, string? filePath, string? email)
        {
            return new Core.Entities.Resume
            {
                UserId = userGuid,
                FatherName = dto.FatherName,
                Position = dto.Position,
                IsDriver = dto.IsDriver,
                IsMarried = dto.IsMarried,
                IsCitizen = dto.IsCitizen,
                MilitarySituation = dto.MilitarySituation,
                IsPublic = dto.IsPublic,
                Gender = dto.Gender,
                Adress = dto.Adress,
                BirthDay = dto.BirthDay,
                UserPhoto = filePath,
                ResumeEmail = email
            };
        }

        private async Task<List<Core.Entities.Number>> GetPhoneNumbersAsync(ResumeCreateDto dto, Guid resumeId, ResumeCreateListsDto listsDto)
        {
            if (!dto.IsMainNumber)
                return await _numberService.CreateBulkNumberAsync(listsDto.NumberCreateDtos.PhoneNumbers, resumeId);

            var mainNumber = (await _userInformationService.GetUserDataAsync(userGuid)).MainPhoneNumber;

            return
            [
                new()
                {
                    PhoneNumber = mainNumber,
                    ResumeId = resumeId
                }
            ];
        }

        private async Task<ICollection<Core.Entities.Certificate>> GetCertificatesAsync(ResumeCreateDto dto)
        {
            return dto.Certificates != null
                ? await _certificateService.CreateBulkCertificateAsync(dto.Certificates)
                : [];
        }

        private static List<ResumeSkill> GetResumeSkills(IEnumerable<Guid>? skillIds, Guid resumeId)
        {
            return skillIds != null
                ? skillIds.Select(skillId => new ResumeSkill
                {
                    SkillId = skillId,
                    ResumeId = resumeId
                }).ToList()
                : [];
        }



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
                ?? throw new NotFoundException<Core.Entities.Resume>(MessageHelper.GetMessage("NOT_FOUND"));
        }

        private static void UpdateResumePersonalInfo(Core.Entities.Resume resume, ResumeUpdateDto updateDto)
        {
            resume.FatherName = updateDto.FatherName;
            resume.Position = updateDto.Position;
            resume.IsDriver = updateDto.IsDriver;
            resume.IsMarried = updateDto.IsMarried;
            resume.IsCitizen = updateDto.IsCitizen;
            resume.MilitarySituation = updateDto.MilitarySituation;
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
                                        .Include(r => r.ResumeSkills).ThenInclude(rs => rs.Skill.Translations)
                                        .Where(x => x.UserId == userGuid)
                                        .Select(resume => new ResumeDetailItemDto
                                        {
                                            UserId = resume.UserId,
                                            FatherName = resume.FatherName,
                                            Position = resume.Position,
                                            IsDriver = resume.IsDriver,
                                            IsMarried = resume.IsMarried,
                                            IsCitizen = resume.IsCitizen,
                                            MilitarySituation = resume.MilitarySituation,
                                            Gender = resume.Gender,
                                            Adress = resume.Adress,
                                            BirthDay = resume.BirthDay,
                                            ResumeEmail = resume.ResumeEmail,
                                            Skills = resume.ResumeSkills.Select(s => new SkillGetByIdDto
                                            {
                                                Id = s.SkillId,
                                                Name = s.Skill.GetTranslation(_currentUser.LanguageCode)
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
                                        ?? throw new NotFoundException<Core.Entities.Resume>(MessageHelper.GetMessage("NOT_FOUND"));

            var userFullName = await _userInformationService.GetUserDataAsync(userGuid);
            resume.FirstName = userFullName.FirstName;
            resume.LastName = userFullName.LastName;

            //var response = await _resumeUser.GetResponse<GetResumeUserPhotoResponse>(new GetResumeUserPhotoRequest
            //{
            //    UserId = userGuid
            //});

            resume.UserPhoto = $"{_authServiceBaseUrl}/{userFullName.ProfileImage}";

            return resume;
        }
    }
}
