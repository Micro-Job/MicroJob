using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.Common;
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
    public class ResumeService(JobDbContext _context,
            IFileService _fileService,
            INumberService _numberService,
            IEducationService _educationService,
            IExperienceService _experienceService,
            ILanguageService _languageService,
            ICertificateService _certificateService,
            IUserInformationService _userInformationService,
            ICurrentUser _currentUser) : IResumeService
    {
        public async Task CreateResumeAsync(ResumeCreateDto resumeCreateDto)
        {
            if (await _context.Resumes.AnyAsync(x => x.UserId == _currentUser.UserGuid))
                throw new IsAlreadyExistException<Core.Entities.Resume>();

            var resume = await BuildResumeAsync(resumeCreateDto);

            await _context.Resumes.AddAsync(resume);
            await _context.SaveChangesAsync();

            var phoneNumbers = await GetPhoneNumbersAsync(resumeCreateDto, resume.Id, resumeCreateDto);
            var educations = await _educationService.CreateBulkEducationAsync(resumeCreateDto.Educations, resume.Id);
            var experiences = await _experienceService.CreateBulkExperienceAsync(resumeCreateDto.Experiences, resume.Id);
            var languages = await _languageService.CreateBulkLanguageAsync(resumeCreateDto.Languages, resume.Id);
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

        public async Task<ResumeDetailItemDto> GetOwnResumeAsync()
        {
            var resume = await _context.Resumes
                                        .Include(x=> x.ResumeSkills).ThenInclude(x=> x.Skill).ThenInclude(x=> x.Translations)
                                        .Where(x => x.UserId == _currentUser.UserGuid)
                                        .Select(resume => new ResumeDetailItemDto
                                        {
                                            UserId = resume.UserId,
                                            FirstName = resume.FirstName,
                                            LastName = resume.LastName,
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
                                            UserPhoto = resume.UserPhoto != null ? $"{_currentUser.BaseUrl}/{resume.UserPhoto}" : null,
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
                                                EducationId = e.Id,
                                                InstitutionName = e.InstitutionName,
                                                Profession = e.Profession,
                                                StartDate = e.StartDate,
                                                EndDate = e.EndDate,
                                                IsCurrentEducation = e.IsCurrentEducation,
                                                ProfessionDegree = e.ProfessionDegree
                                            }).ToList(),
                                            Experiences = resume.Experiences.Select(ex => new ExperienceGetByIdDto
                                            {
                                                ExperienceId = ex.Id,
                                                OrganizationName = ex.OrganizationName,
                                                PositionName = ex.PositionName,
                                                PositionDescription = ex.PositionDescription,
                                                StartDate = ex.StartDate,
                                                EndDate = ex.EndDate,
                                                IsCurrentOrganization = ex.IsCurrentOrganization
                                            }).ToList(),
                                            Languages = resume.Languages.Select(l => new LanguageGetByIdDto
                                            {
                                                LanguageId = l.Id,
                                                LanguageName = l.LanguageName,
                                                LanguageLevel = l.LanguageLevel
                                            }).ToList(),
                                            Certificates = resume.Certificates.Select(c => new CertificateGetByIdDto
                                            {
                                                CertificateId = c.Id,
                                                CertificateName = c.CertificateName,
                                                GivenOrganization = c.GivenOrganization,
                                                CertificateFile = $"{_currentUser.BaseUrl}/{c.CertificateFile}"
                                            }).ToList()
                                        })
                                        .FirstOrDefaultAsync();

            if (resume is null) return new ResumeDetailItemDto();

            var userData = await _userInformationService.GetUserDataAsync((Guid)_currentUser.UserGuid);
            resume.FirstName = userData.FirstName;
            resume.LastName = userData.LastName;

            return resume;
        }

        public async Task UpdateResumeAsync(ResumeUpdateDto updateDto)
        {
            var resume = await GetResumeByUserIdAsync((Guid)_currentUser.UserGuid);

            UpdateResumePersonalInfo(resume, updateDto);

            if (updateDto.UserPhoto != null) UpdateUserPhotoAsync(resume, updateDto.UserPhoto);

            await UpdateResumeEmailAsync(resume, updateDto.IsMainEmail, updateDto.ResumeEmail);

            await UpdatePhoneNumbersAsync(resume, updateDto.PhoneNumbers, updateDto.IsMainNumber);

            resume.Educations = await _educationService.UpdateBulkEducationAsync(updateDto.Educations, resume.Id);
            resume.Experiences = await _experienceService.UpdateBulkExperienceAsync(updateDto.Experiences, resume.Id);
            resume.Languages = await _languageService.UpdateBulkLanguageAsync(updateDto.Languages, resume.Id);
            resume.Certificates = await UpdateCertificatesAsync(updateDto.Certificates ?? []);

            UpdateResumeSkills(resume, updateDto.SkillIds);

            await _context.SaveChangesAsync();
        }

        //Sirket hissəsində resumelerin metodlari
        public async Task<DataListDto<ResumeListDto>> GetAllResumesAsync(string? fullname, int skip, int take)
        {
            var query = _context.Resumes.Where(x=> x.IsPublic).AsQueryable().AsNoTracking();

            if (fullname != null)
                query = query.Where(x=> (x.FirstName + x.LastName).Contains(fullname));

            var resumes = await query.Select(x => new ResumeListDto
            {
                Id = x.Id,
                FullName = $"{x.FirstName} {x.LastName}",
                ProfileImage = x.UserPhoto != null ? $"{_currentUser.BaseUrl}/{x.UserPhoto}" : null,
                JobStatus = x.User.JobStatus,
                IsSaved = x.SavedResumes.Any(sr => sr.ResumeId == x.Id && sr.CompanyUserId == _currentUser.UserGuid),
                Position = x.Position,
                StartDate = x.Experiences != null ? x.Experiences.FirstOrDefault().StartDate : null,  
                EndDate = x.Experiences != null ? x.Experiences.FirstOrDefault().EndDate : null,  
                //SkillsName = x.ResumeSkills.Select(x=> new
                //{
                //    x.
                //})
            })
            .Skip(Math.Max(0, (skip - 1) * take))
            .Take(take) 
            .ToListAsync();

            return new DataListDto<ResumeListDto> 
            {
                Datas = resumes,
                TotalCount = await query.CountAsync()
            };
        }

        public async Task<DataListDto<ResumeListDto>> GetSavedResumesAsync(string? fullName, int skip, int take)
        {
            var query = _context.SavedResumes.Where(x => x.CompanyUserId == _currentUser.UserGuid);

            var resumes = await query.Select(x => new ResumeListDto
            {
                Id = x.Id,
                FullName = $"{x.Resume.FirstName} {x.Resume.LastName}",
                ProfileImage = x.Resume.UserPhoto != null ? $"{_currentUser.BaseUrl}/{x.Resume.UserPhoto}" : null,
                JobStatus = x.Resume.User.JobStatus,
                IsSaved = true,
                Position = x.Resume.Position,
                StartDate = x.Resume.Experiences != null ? x.Resume.Experiences.FirstOrDefault().StartDate : null,
                EndDate = x.Resume.Experiences != null ? x.Resume.Experiences.FirstOrDefault().EndDate : null,
            })
            .Skip(Math.Max(0, (skip - 1) * take))
            .Take(take)
            .ToListAsync();

            return new DataListDto<ResumeListDto>
            {
                Datas = resumes,
                TotalCount = await query.CountAsync()
            };
        }

        public async Task ToggleSaveResumeAsync(string resumeId)
        {
            var resumeGuid = Guid.Parse(resumeId);

            if (!await _context.Resumes.AnyAsync(x => x.Id == resumeGuid && x.IsPublic)) 
                throw new NotFoundException<Core.Entities.Resume>("CV mövcud deyil");

            var existSaveResume = await _context.SavedResumes.FirstOrDefaultAsync(x => x.ResumeId == resumeGuid && x.CompanyUserId == _currentUser.UserGuid);

            if(existSaveResume == null)
            {
                await _context.SavedResumes.AddAsync(new SavedResume
                {
                    CompanyUserId = (Guid)_currentUser.UserGuid,
                    ResumeId = resumeGuid
                });
            }
            else
            {
                _context.SavedResumes.Remove(existSaveResume);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsExistResumeAsync()
        {
            return await _context.Resumes.AnyAsync(x => x.UserId == _currentUser.UserGuid);
        }

        #region Private Methods
        private async Task<Core.Entities.Resume> BuildResumeAsync(ResumeCreateDto dto)
        {
            FileDto fileResult = dto.UserPhoto != null
                ? await _fileService.UploadAsync(FilePaths.document, dto.UserPhoto)
                : new FileDto();

            string? email = dto.IsMainEmail
                ? (await _userInformationService.GetUserDataAsync((Guid)_currentUser.UserGuid)).Email
                : dto.ResumeEmail;

            return MapToResumeEntity(dto, $"{fileResult.FilePath}/{fileResult.FileName}", email);
        }

        private Core.Entities.Resume MapToResumeEntity(ResumeCreateDto dto, string? filePath, string? email)
        {
            return new Core.Entities.Resume
            {
                UserId = (Guid)_currentUser.UserGuid,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
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

        private async Task<List<Core.Entities.Number>> GetPhoneNumbersAsync(ResumeCreateDto dto, Guid resumeId, ResumeCreateDto listsDto)
        {
            if (!dto.IsMainNumber)
                return await _numberService.CreateBulkNumberAsync(listsDto.PhoneNumbers, resumeId);

            var mainNumber = (await _userInformationService.GetUserDataAsync((Guid)_currentUser.UserGuid)).MainPhoneNumber;

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

        private async Task UpdateResumeEmailAsync(Core.Entities.Resume resume, bool IsMainEmail , string? resumeEmail)
        {
            resume.ResumeEmail = IsMainEmail
                ? (await _userInformationService.GetUserDataAsync((Guid)_currentUser.UserGuid)).Email
                : resumeEmail;
        }

        private async Task UpdatePhoneNumbersAsync(Core.Entities.Resume resume, ICollection<NumberUpdateDto> phoneNumbers, bool isMainNumber)
        {
            if (isMainNumber)
            {
                var mainNumber = new List<Core.Entities.Number>
                {
                    new() {
                        PhoneNumber = _userInformationService.GetUserDataAsync((Guid)_currentUser.UserGuid).Result.MainPhoneNumber,
                        ResumeId = resume.Id
                    }
                };

                resume.PhoneNumbers = mainNumber;
            }
            else
            {
                _context.Numbers.RemoveRange(resume.PhoneNumbers);
                resume.PhoneNumbers = await _numberService.UpdateBulkNumberAsync(phoneNumbers, resume.Id);
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

        
        #endregion
    }
}
