using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.Common;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Dtos.ResumeDtos;
using Job.Business.Dtos.SkillDtos;
using Job.Business.Exceptions.Common;
using Job.Business.Extensions;
using Job.Business.Services.Certificate;
using Job.Business.Services.Education;
using Job.Business.Services.Experience;
using Job.Business.Services.Language;
using Job.Business.Services.Number;
using Job.Business.Services.Position;
using Job.Business.Services.User;
using Job.Business.Statistics;
using Job.Core.Entities;
using Job.Core.Enums;
using Job.DAL.Contexts;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Shared.Enums;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.Enums;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Statics;

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
            ICurrentUser _currentUser,
            IPositionService _positionService) : IResumeService
    {
        public async Task CreateResumeAsync(ResumeCreateDto resumeCreateDto)
        {
            if (await _context.Resumes.AnyAsync(x => x.UserId == _currentUser.UserGuid))
                throw new IsAlreadyExistException<Core.Entities.Resume>();

            var positionId = await _positionService.GetOrCreatePositionAsync(resumeCreateDto.Position, resumeCreateDto.PositionId, resumeCreateDto.ParentPositionId);

            var resume = await BuildResumeAsync(resumeCreateDto, positionId);

            if (positionId != Guid.Empty)
                resume.PositionId = positionId;


            await _context.Resumes.AddAsync(resume);
            await _context.SaveChangesAsync();

            var phoneNumbers = await GetPhoneNumbersAsync(resumeCreateDto, resume.Id, resumeCreateDto);
            var educations = resumeCreateDto.Educations != null
                ? await _educationService.CreateBulkEducationAsync(resumeCreateDto.Educations, resume.Id)
                : [];
            var experiences = resumeCreateDto.Experiences != null
                ? await _experienceService.CreateBulkExperienceAsync(resumeCreateDto.Experiences, resume.Id)
                : [];
            var languages = resumeCreateDto.Languages != null
                ? await _languageService.CreateBulkLanguageAsync(resumeCreateDto.Languages, resume.Id)
                : [];
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
                                        .Include(x => x.ResumeSkills).ThenInclude(x => x.Skill).ThenInclude(x => x.Translations)
                                        .Where(x => x.UserId == _currentUser.UserGuid)
                                        .Select(resume => new ResumeDetailItemDto
                                        {
                                            UserId = resume.UserId,
                                            FirstName = resume.FirstName,
                                            LastName = resume.LastName,
                                            FatherName = resume.FatherName,
                                            Position = resume.Position != null ? resume.Position.Name : null,
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
                                                Name = s.Skill.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name)
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

            var positionId = await _positionService.GetOrCreatePositionAsync(updateDto.Position, updateDto.PositionId);

            if (positionId != Guid.Empty)
                resume.PositionId = positionId;

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
        public async Task<DataListDto<ResumeListDto>> GetAllResumesAsync(
            string? fullname,
            bool? isPublic,
            ProfessionDegree? professionDegree,
            Citizenship? citizenship,
            bool? isExperience,
            JobStatus? jobStatus,
            List<string>? skillIds,
            List<LanguageFilterDto>? languages,
            int skip,
            int take)
        {
            var query = _context.Resumes
                .Include(r => r.ResumeSkills)
                .ThenInclude(rs => rs.Skill.Translations)
                .Include(r => r.Languages)
                .AsQueryable()
                .AsNoTracking();

            query = ApplyFilters(query, fullname, isPublic, professionDegree, citizenship, isExperience, skillIds, languages);

            var resumes = await query.Select(x => new ResumeListDto
            {
                Id = x.Id,
                FullName = x.IsPublic ? $"{x.FirstName} {x.LastName}" : null,
                ProfileImage = x.IsPublic && x.UserPhoto != null
                ? $"{_currentUser.BaseUrl}/{x.UserPhoto}"
                : null,
                IsSaved = x.SavedResumes.Any(sr => sr.ResumeId == x.Id && sr.CompanyUserId == _currentUser.UserGuid),
                JobStatus = x.User.JobStatus,
                LastWork = x.Experiences
                    .OrderByDescending(e => e.StartDate)
                    .Select(e => new LastWorkDto
                    {
                        CompanyName = e.OrganizationName,
                        Position = e.PositionName,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate
                    })
                    .FirstOrDefault(),
                SkillsName = x.ResumeSkills
                .Select(s => s.Skill.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name))
                .ToList(),
                Position = x.Position != null ? x.Position.Name : null,

            })
            .Skip(Math.Max(0, (skip - 1) * take))
            .Take(take)
            .ToListAsync();

            var totalCount = await query.CountAsync();

            return new DataListDto<ResumeListDto>
            {
                Datas = resumes,
                TotalCount = totalCount
            };
        }



        private IQueryable<Core.Entities.Resume> ApplyFilters(
        IQueryable<Core.Entities.Resume> query,
        string? fullname,
        bool? isPublic,
        ProfessionDegree? professionDegree,
        Citizenship? citizenship,
        bool? isExperience,
        List<string>? skillIds,
        List<LanguageFilterDto>? languages)
        {
            if (isPublic.HasValue)
            {
                query = query.Where(x => x.IsPublic == isPublic.Value);
            }

            if (!string.IsNullOrEmpty(fullname))
            {
                query = query.Where(x => (x.FirstName + " " + x.LastName).Contains(fullname));
            }

            if (professionDegree != null)
            {
                query = query.Where(x => x.Educations.Any(e => e.ProfessionDegree == professionDegree));
            }

            if (citizenship != null)
            {
                query = query.Where(x => x.IsCitizen == citizenship);
            }

            if (isExperience.HasValue)
            {
                if (isExperience.Value)
                    query = query.Where(x => x.Experiences.Any());
                else
                    query = query.Where(x => !x.Experiences.Any());
            }

            if (skillIds != null && skillIds.Any())
            {
                query = query.Where(x => x.ResumeSkills.Any(rs => skillIds.Contains(rs.SkillId.ToString())));
            }

            if (languages != null && languages.Any())
            {
                query = query.Where(x => languages.All(lang =>
                    x.Languages.Any(rl =>
                        rl.LanguageName == lang.Language &&
                        rl.LanguageLevel == lang.LanguageLevel)));
            }

            return query;
        }


        public async Task<DataListDto<ResumeListDto>> GetSavedResumesAsync(string? fullName, int skip, int take)
        {
            var query = _context.SavedResumes.Include(sr => sr.Resume).ThenInclude(r => r.ResumeSkills).ThenInclude(rs => rs.Skill.Translations).Where(x => x.CompanyUserId == _currentUser.UserGuid);

            var resumes = await query.Select(x => new ResumeListDto
            {
                Id = x.Id,
                FullName = x.Resume.IsPublic ? $"{x.Resume.FirstName} {x.Resume.LastName}" : null,
                ProfileImage = x.Resume.IsPublic && x.Resume.UserPhoto != null
                ? $"{_currentUser.BaseUrl}/{x.Resume.UserPhoto}"
                : null,
                IsSaved = x.Resume.SavedResumes.Any(sr => sr.ResumeId == x.Id && sr.CompanyUserId == _currentUser.UserGuid),
                JobStatus = x.Resume.User.JobStatus,
                LastWork = x.Resume.Experiences
                    .OrderByDescending(e => e.StartDate)
                    .Select(e => new LastWorkDto
                    {
                        CompanyName = e.OrganizationName,
                        Position = e.PositionName,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate
                    })
                    .FirstOrDefault(),
                SkillsName = x.Resume.ResumeSkills
                .Select(s => s.Skill.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name))
                .ToList(),
                Position = x.Resume.Position != null ? x.Resume.Position.Name : null,

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

            if (!await _context.Resumes.AnyAsync(x => x.Id == resumeGuid))
                throw new NotFoundException<Core.Entities.Resume>("CV mövcud deyil");

            var existSaveResume = await _context.SavedResumes.FirstOrDefaultAsync(x => x.ResumeId == resumeGuid && x.CompanyUserId == _currentUser.UserGuid);

            if (existSaveResume == null)
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
        private async Task<Core.Entities.Resume> BuildResumeAsync(ResumeCreateDto dto, Guid positionId)
        {
            FileDto fileResult = dto.UserPhoto != null
                ? await _fileService.UploadAsync(FilePaths.image, dto.UserPhoto)
                : new FileDto();

            string? email = dto.IsMainEmail
                ? (await _userInformationService.GetUserDataAsync((Guid)_currentUser.UserGuid)).Email
                : dto.ResumeEmail;

            return MapToResumeEntity(dto, $"{fileResult.FilePath}/{fileResult.FileName}", email, positionId);
        }

        private Core.Entities.Resume MapToResumeEntity(ResumeCreateDto dto, string? filePath, string? email, Guid positionId)
        {
            return new Core.Entities.Resume
            {
                UserId = (Guid)_currentUser.UserGuid,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                FatherName = dto.FatherName,
                PositionId = dto.PositionId,
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

        private async Task UpdateResumeEmailAsync(Core.Entities.Resume resume, bool IsMainEmail, string? resumeEmail)
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

        private async Task<ICollection<Core.Entities.Certificate>> UpdateCertificatesAsync(ICollection<CertificateUpdateDto>? certificatesDto)
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

        public async Task<ResumeDetailItemDto> GetByIdResumeAysnc(string id)
        {
            var userId = _currentUser.UserGuid;
            var resumeGuid = Guid.Parse(id);
            var resume = await _context.Resumes.Where(r => r.Id == resumeGuid)
            .Include(x => x.ResumeSkills)
                .ThenInclude(x => x.Skill)
                    .ThenInclude(x => x.Translations)
            .Include(x => x.SavedResumes)
            .Select(r => new ResumeDetailItemDto
            {
                UserId = r.UserId,
                ResumeId = r.Id,
                IsSaved = r.SavedResumes.Any(sr => sr.CompanyUserId == userId),
                FirstName = r.IsPublic ? r.FirstName : null,
                LastName = r.IsPublic ? r.LastName : null,
                FatherName = r.FatherName,
                IsDriver = r.IsDriver,
                IsMarried = r.IsMarried,
                IsCitizen = r.IsCitizen,
                MilitarySituation = r.MilitarySituation,
                Gender = r.Gender,
                Adress = r.Adress,
                Position = r.Position != null ? r.Position.Name : null,
                BirthDay = r.BirthDay,
                ResumeEmail = r.IsPublic ? r.ResumeEmail : null,
                UserPhoto = r.IsPublic && r.UserPhoto != null ? $"{_currentUser.BaseUrl}/{r.UserPhoto}" : null,
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
            .FirstOrDefaultAsync() ?? throw new NotFoundException<Core.Entities.Resume>("Resume mövcud deyil");

            return resume;
        }


        #endregion
    }
}