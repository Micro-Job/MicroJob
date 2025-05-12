using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.Common;
using Job.Business.Dtos.EducationDtos;
using Job.Business.Dtos.ExperienceDtos;
using Job.Business.Dtos.LanguageDtos;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Dtos.ResumeDtos;
using Job.Business.Dtos.SkillDtos;
using Job.Business.Exceptions.Common;
using Job.Business.Exceptions.ResumeExceptions;
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
using MassTransit;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.Enums;
using SharedLibrary.Events;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
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
            IPositionService _positionService,
            IPublishEndpoint _publishEndpoint,
            IRequestClient<CheckBalanceRequest> _balanceRequest,
            IRequestClient<CheckApplicationRequest> _checkApplicationRequest) : IResumeService
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
            var userData = await _userInformationService.GetUserDataAsync((Guid)_currentUser.UserGuid);

            var userMainEmail = userData.Email;
            var userMainPhoneNumber = userData.MainPhoneNumber;

            var resume = await _context.Resumes
                                        .Include(x => x.ResumeSkills).ThenInclude(x => x.Skill).ThenInclude(x => x.Translations)
                                        .Where(x => x.UserId == _currentUser.UserGuid)
                                        .Select(resume => new ResumeDetailItemDto
                                        {
                                            ResumeId = resume.Id,
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
                                            IsMainEmail = resume.ResumeEmail == userMainEmail,
                                            IsMainNumber = resume.PhoneNumbers.Any(p => p.PhoneNumber == userMainPhoneNumber),
                                            PositionId = resume.PositionId,
                                            ParentPositionId = resume.Position != null ? resume.Position.ParentPositionId : null,
                                            UserPhoto = resume.UserPhoto != null ? $"{_currentUser.BaseUrl}/{resume.UserPhoto}" : null,
                                            IsPublic = resume.IsPublic,
                                            IsAnonym = resume.IsAnonym,
                                            Skills = resume.ResumeSkills.Select(s => new SkillGetByIdDto
                                            {
                                                Id = s.SkillId,
                                                Name = s.Skill.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name)
                                            }).ToList(),
                                            PhoneNumbers = resume.PhoneNumbers.Select(p => new NumberGetByIdDto
                                            {
                                                PhoneNumberId = p.Id,
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

            //resume.FirstName = userData.FirstName;
            //resume.LastName = userData.LastName;

            return resume;
        }

        public async Task UpdateResumeAsync(ResumeUpdateDto updateDto)
        {
            var resume = await GetResumeByUserIdAsync((Guid)_currentUser.UserGuid);

            UpdateResumePersonalInfo(resume, updateDto);

            var positionId = await _positionService.GetOrCreatePositionAsync(updateDto.Position, updateDto.PositionId, updateDto.ParentPositionId);

            if (positionId != Guid.Empty)
                resume.PositionId = positionId;

            if (updateDto.UserPhoto != null) UpdateUserPhotoAsync(resume, updateDto.UserPhoto);

            await UpdateResumeEmailAsync(resume, updateDto.IsMainEmail, updateDto.ResumeEmail);

            await UpdatePhoneNumbersAsync(resume, updateDto.PhoneNumbers, updateDto.IsMainNumber);

            resume.Educations = updateDto.Educations != null
                ? await _educationService.UpdateBulkEducationAsync(updateDto.Educations, resume.Educations, resume.Id)
                : [];

            resume.Experiences = updateDto.Experiences != null
                ? await _experienceService.UpdateBulkExperienceAsync(updateDto.Experiences, resume.Experiences, resume.Id)
                : [];

            resume.Languages = updateDto.Languages != null
                ? await _languageService.UpdateBulkLanguageAsync(updateDto.Languages, resume.Id)
                : [];

            resume.Certificates = updateDto.Certificates != null
                ? await _certificateService.UpdateBulkCertificateAsync(updateDto.Certificates, resume.Certificates)
                : [];

            UpdateResumeSkills(resume, updateDto.SkillIds);

            await _context.SaveChangesAsync();
        }

        //Sirket hissəsində resumelerin metodlari
        public async Task<DataListDto<ResumeListDto>> GetAllResumesAsync(string? fullname, bool? isPublic, ProfessionDegree? professionDegree, Citizenship? citizenship, bool? isExperience, JobStatus? jobStatus, List<string>? skillIds, List<LanguageFilterDto>? languages, int skip, int take)
        {
            var query = _context.Resumes
                .Where(r => !r.IsAnonym)
                .Include(r => r.ResumeSkills)
                .ThenInclude(rs => rs.Skill.Translations)
                .Include(r => r.Languages)
                .Include(r => r.CompanyResumeAccesses)
                .AsNoTracking();

            query = ApplyFilters(query, fullname, isPublic, professionDegree, citizenship, isExperience, skillIds, languages, jobStatus);

            var resumes = await query.Select(x => new ResumeListDto
            {
                Id = x.Id,
                FullName = x.IsPublic ? $"{x.FirstName} {x.LastName}" : x.CompanyResumeAccesses.Any(cra => cra.CompanyUserId == _currentUser.UserGuid && cra.ResumeId == x.Id) ? $"{x.FirstName} {x.LastName}" : null,
                ProfileImage = x.UserPhoto != null ? x.IsPublic ? $"{_currentUser.BaseUrl}/{x.UserPhoto}" : x.CompanyResumeAccesses.Any(cra => cra.CompanyUserId == _currentUser.UserGuid && cra.ResumeId == x.Id) ? $"{_currentUser.BaseUrl}/{x.UserPhoto}" : null : null,
                IsSaved = x.SavedResumes.Any(sr => sr.ResumeId == x.Id && sr.CompanyUserId == _currentUser.UserGuid),
                IsPublic = x.IsPublic,
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
                HasAccess = x.IsPublic || x.CompanyResumeAccesses.Any(cra => cra.CompanyUserId == _currentUser.UserGuid)
            })
            .Skip((skip - 1) * take)
            .Take(take)
            .ToListAsync();

            var totalCount = await query.CountAsync();

            return new DataListDto<ResumeListDto>
            {
                Datas = resumes,
                TotalCount = totalCount
            };
        }

        private IQueryable<Core.Entities.Resume> ApplyFilters(IQueryable<Core.Entities.Resume> query, string? fullname, bool? isPublic, ProfessionDegree? professionDegree, Citizenship? citizenship, bool? isExperience, List<string>? skillIds, List<LanguageFilterDto>? languages, JobStatus? jobStatus)
        {
            if (isPublic != null)
            {
                query = query.Where(x => x.IsPublic == isPublic);
            }

            if (jobStatus != null)
            {
                query = query.Where(x => x.User.JobStatus == jobStatus);
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
                foreach (var lang in languages)
                {
                    query = query.Where(x => x.Languages.Any(rl =>
                        rl.LanguageName == lang.Language &&
                        rl.LanguageLevel == lang.LanguageLevel));
                }
            }

            return query;
        }

        public async Task<DataListDto<ResumeListDto>> GetSavedResumesAsync(string? fullName, bool? isPublic, JobStatus? jobStatus, ProfessionDegree? professionDegree, Citizenship? citizenship, bool? isExperience, List<string>? skillIds, List<LanguageFilterDto>? languages, int skip, int take)
        {
            var resumeQuery = _context.SavedResumes
                .Where(sr => sr.CompanyUserId == _currentUser.UserGuid)
                .Include(sr => sr.Resume)
                    .ThenInclude(r => r.ResumeSkills)
                        .ThenInclude(rs => rs.Skill.Translations)
                .Include(sr => sr.Resume.Languages)
                .Include(sr => sr.Resume.CompanyResumeAccesses)
                .Select(sr => sr.Resume)
                .AsNoTracking();

            resumeQuery = ApplyFilters(resumeQuery, fullName, isPublic, professionDegree, citizenship, isExperience, skillIds, languages, jobStatus);

            var resumes = await resumeQuery
               .Select(x => new ResumeListDto
               {
                   Id = x.Id,
                   FullName = x.IsPublic ? $"{x.FirstName} {x.LastName}" : x.CompanyResumeAccesses.Any(cra => cra.CompanyUserId == _currentUser.UserGuid) ? $"{x.FirstName} {x.LastName}" : null,
                   ProfileImage = x.UserPhoto != null
                       ? x.IsPublic || x.CompanyResumeAccesses.Any(cra => cra.CompanyUserId == _currentUser.UserGuid)
                           ? $"{_currentUser.BaseUrl}/{x.UserPhoto}"
                           : null
                       : null,
                   IsSaved = true,
                   IsPublic = x.IsPublic,
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
                   HasAccess = x.IsPublic || x.CompanyResumeAccesses.Any(cra => cra.CompanyUserId == _currentUser.UserGuid)
               })
               .Skip(Math.Max(0, (skip - 1) * take))
               .Take(take)
               .ToListAsync();

            return new DataListDto<ResumeListDto>
            {
                Datas = resumes,
                TotalCount = await resumeQuery.CountAsync()
            };
        }

        public async Task ToggleSaveResumeAsync(string resumeId)
        {
            var resumeGuid = Guid.Parse(resumeId);

            if (!await _context.Resumes.AnyAsync(x => x.Id == resumeGuid))
                throw new NotFoundException<Core.Entities.Resume>();

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

        public async Task<ResumeDetailItemDto> GetByIdResumeAysnc(string id)
        {
            var userId = _currentUser.UserGuid;
            var resumeGuid = Guid.Parse(id);

            var resumeData = await _context.Resumes
                .Where(r => r.Id == resumeGuid)
                .Include(r => r.SavedResumes)
                .Include(r => r.Position)
                .Include(r => r.PhoneNumbers)
                .Include(r => r.Educations)
                .Include(r => r.Experiences)
                .Include(r => r.Languages)
                .Include(r => r.Certificates)
                .Include(r => r.ResumeSkills)
                    .ThenInclude(rs => rs.Skill).ThenInclude(s => s.Translations)
                .Select(r => new
                {
                    Resume = r,
                    ResumeUserId = r.UserId,
                    HasAccessByCompany = r.CompanyResumeAccesses.Any(x => x.CompanyUserId == userId),
                    r.IsPublic
                })
                .FirstOrDefaultAsync() ?? throw new NotFoundException<Core.Entities.Resume>();

            bool hasApplied = false;

            // Əgər CompanyUser-dursa, şirkətin hər hansı bir vakansiyasına müraciət edib-etmədiyini yoxlayırıq
            if (_currentUser.UserRole == (byte)UserRole.CompanyUser)
            {
                var checkApplicationResponse = await _checkApplicationRequest.GetResponse<CheckApplicationResponse>(
                    new CheckApplicationRequest
                    {
                        CompanyUserId = (Guid)userId,
                        UserId = resumeData.ResumeUserId
                    });

                hasApplied = checkApplicationResponse.Message.HasApplied;
            }

            // İcazə olub-olmadığını yoxlayırıq. 4 halda resume-nin bütün datalarına tam baxma icazəsi var:
            bool hasFullAccess = _currentUser.UserRole == (byte)UserRole.Admin // Əgər admindirsə
                || hasApplied                                                  // Əgər müraciət edibsə
                || resumeData.HasAccessByCompany                               // Əgər resume-yə baxma icazəsi varsa
                || resumeData.IsPublic;                                        // Əgər resume public-dirsə

            var resume = resumeData.Resume;

            return new ResumeDetailItemDto
            {
                ResumeId = resume.Id,
                UserId = resume.UserId,
                IsSaved = resume.SavedResumes.Any(sr => sr.ResumeId == resume.Id && sr.CompanyUserId == _currentUser.UserGuid),
                FirstName = hasFullAccess ? resume.FirstName : null,
                LastName = hasFullAccess ? resume.LastName : null,
                FatherName = hasFullAccess ? resume.FatherName : null,
                ResumeEmail = hasFullAccess ? resume.ResumeEmail : null,

                PhoneNumbers = hasFullAccess
                    ? resume.PhoneNumbers.Select(p => new NumberGetByIdDto
                    {
                        PhoneNumberId = p.Id,
                        PhoneNumber = p.PhoneNumber
                    }).ToList()
                    : [],

                UserPhoto = hasFullAccess && resume.UserPhoto != null
                    ? $"{_currentUser.BaseUrl}/{resume.UserPhoto}"
                    : null,

                BirthDay = resume.BirthDay,
                Gender = resume.Gender,
                IsMarried = resume.IsMarried,
                IsDriver = resume.IsDriver,
                IsCitizen = resume.IsCitizen,
                MilitarySituation = resume.MilitarySituation,
                Adress = resume.Adress,
                Position = resume.Position?.Name,

                Skills = resume.ResumeSkills.Select(s => new SkillGetByIdDto
                {
                    Id = s.SkillId,
                    Name = s.Skill.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name)
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
            };
        }



        public async Task TakeResumeAccessAsync(string resumeId)
        {
            var resumeGuid = Guid.Parse(resumeId);

            if (await _context.CompanyResumeAccesses.AnyAsync(x => x.ResumeId == resumeGuid && x.CompanyUserId == _currentUser.UserGuid))
                throw new IsAlreadyExistException<CompanyResumeAccess>(MessageHelper.GetMessage("RESUME_ACCESS_ALREADY_EXIST"));

            var resume = await _context.Resumes.Where(x => x.Id == resumeGuid)
                .Select(x => new
                {
                    x.IsPublic
                }).FirstOrDefaultAsync();

            if (resume == null)
                throw new NotFoundException<Core.Entities.Resume>();

            if (resume.IsPublic) throw new ResumeIsPublicException();

            //TODO : bu hisse olmaya da biler.Bir basa exception qaytararaq
            var checkBalanceResponse = await _balanceRequest.GetResponse<CheckBalanceResponse>(new CheckBalanceRequest
            {
                InformationType = InformationType.AnonymResume,
                UserId = (Guid)_currentUser.UserGuid
            });
            if (checkBalanceResponse.Message.HasEnoughBalance)
            {
                //Odenisin olmasi ucun
                await _publishEndpoint.Publish(new PayEvent
                {
                    InformationType = InformationType.AnonymResume,
                    InformationId = resumeGuid,
                    UserId = (Guid)_currentUser.UserGuid
                });
            }
            //TODO : Burada exception qaytarmaq prinsip olaraq dogru deyil
            else throw new NotFoundException<Core.Entities.Resume>("Yeterli balans yoxdur");

            await _context.CompanyResumeAccesses.AddAsync(new CompanyResumeAccess
            {
                AccessDate = DateTime.Now,
                CompanyUserId = (Guid)_currentUser.UserGuid,
                ResumeId = resumeGuid
            });

            await _context.SaveChangesAsync();
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
                ResumeEmail = email,
                IsAnonym = dto.IsAnonym
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
                .Include(r => r.ResumeSkills)
                .FirstOrDefaultAsync(r => r.UserId == userGuid)
                ?? throw new NotFoundException<Core.Entities.Resume>();
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
            resume.IsAnonym = updateDto.IsAnonym;
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
                resume.PhoneNumbers = await _numberService.UpdateBulkNumberAsync(phoneNumbers, resume.PhoneNumbers, resume.Id);
            }
        }

        private static void UpdateResumeSkills(Core.Entities.Resume resume, IEnumerable<Guid>? skillIds)
        {
            resume.ResumeSkills.Clear();

            resume.ResumeSkills = skillIds?.Distinct().Select(skillId => new ResumeSkill
            {
                SkillId = skillId,
                ResumeId = resume.Id
            }).ToList() ?? [];
        }

        #endregion
    }
}