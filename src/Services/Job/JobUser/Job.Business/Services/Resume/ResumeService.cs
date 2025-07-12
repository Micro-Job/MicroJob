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
using SharedLibrary.Exceptions;
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
            NumberService _numberService,
            EducationService _educationService,
            ExperienceService _experienceService,
            LanguageService _languageService,
            CertificateService _certificateService,
            ICurrentUser _currentUser,
            PositionService _positionService,
            IPublishEndpoint _publishEndpoint,
            IRequestClient<CheckBalanceRequest> _balanceRequest,
            IRequestClient<CheckApplicationRequest> _checkApplicationRequest)
    {
        public async Task<string> CreateResumeAsync(ResumeCreateDto resumeCreateDto)
        {
            if (await _context.Resumes.AnyAsync(x => x.UserId == _currentUser.UserGuid))
                throw new IsAlreadyExistException<Core.Entities.Resume>();

            var positionId = await _positionService.GetOrCreatePositionAsync(resumeCreateDto.Position, resumeCreateDto.PositionId, resumeCreateDto.ParentPositionId);

            var resume = await BuildResumeAsync(resumeCreateDto, positionId);

            await _context.Resumes.AddAsync(resume);
            await _context.SaveChangesAsync();

            var phoneNumbers = await GetPhoneNumbersAsync(resumeCreateDto, resume.Id);
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

            var user = await _context.Users.FirstOrDefaultAsync(x=> x.Id == _currentUser.UserGuid) ?? throw new NotFoundException();

            if(user.Image == null)
            {
                FileDto fileResult = resumeCreateDto.UserPhoto != null
                    ? await _fileService.UploadAsync(FilePaths.image, resumeCreateDto.UserPhoto, FileType.Image)
                    : new FileDto();

                user.Image = $"{fileResult.FilePath}/{fileResult.FileName}";
            }

            await _context.SaveChangesAsync();

            return $"{_currentUser.BaseUrl}/userFiles/{user.Image}";
        }

        public async Task<ResumeDetailItemDto> GetOwnResumeAsync()
        {
            var resume = await _context.Resumes
                                        .Where(x => x.UserId == _currentUser.UserGuid)
                                        .Select(resume => new ResumeDetailItemDto
                                        {
                                            //ResumeId = resume.Id,
                                            //UserId = resume.UserId,
                                            FirstName = resume.FirstName,
                                            LastName = resume.LastName,
                                            Position = resume.Position != null ? resume.Position.Name : null,
                                            IsDriver = resume.IsDriver,
                                            IsMarried = resume.IsMarried,
                                            IsCitizen = resume.IsCitizen,
                                            MilitarySituation = resume.MilitarySituation,
                                            Gender = resume.Gender,
                                            Adress = resume.Adress,
                                            BirthDay = resume.BirthDay,
                                            ResumeEmail = resume.ResumeEmail,
                                            IsMainEmail = resume.ResumeEmail == resume.User.Email,
                                            IsMainNumber = resume.PhoneNumbers.Any(p => p.PhoneNumber == resume.User.MainPhoneNumber),
                                            PositionId = resume.PositionId,
                                            ParentPositionId = resume.Position != null ? resume.Position.ParentPositionId : null,
                                            UserPhoto = resume.UserPhoto != null ? $"{_currentUser.BaseUrl}/userFiles/{resume.UserPhoto}" : null,
                                            IsPublic = resume.IsPublic,
                                            IsAnonym = resume.IsAnonym,
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
                                                CertificateFile = $"{_currentUser.BaseUrl}/userFiles/{c.CertificateFile}"
                                            }).ToList()
                                        })
                                        .FirstOrDefaultAsync();

            if (resume is null) return new ResumeDetailItemDto();

            resume.Skills = await _context.ResumeSkills.Where(x=> x.Resume.UserId == _currentUser.UserGuid)
            .Select(x => new SkillGetByIdDto
            {
                Id = x.SkillId,
                Name = x.Skill.Translations.Where(x => x.Language == _currentUser.LanguageCode).Select(x => x.Name).FirstOrDefault()
            }).ToListAsync();

            return resume;
        }

        public async Task UpdateResumeAsync(ResumeUpdateDto updateDto)
        {
            var resume = await GetResumeByUserIdAsync((Guid)_currentUser.UserGuid!);

            UpdateResumePersonalInfo(resume, updateDto);

            var positionId = await _positionService.GetOrCreatePositionAsync(updateDto.Position, updateDto.PositionId, updateDto.ParentPositionId);

            if (positionId != Guid.Empty)
                resume.PositionId = positionId;

            if (updateDto.UserPhoto != null) await UpdateUserPhotoAsync(resume, updateDto.UserPhoto);

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
                : _certificateService.DeleteAllCertificates(resume.Certificates);

            UpdateResumeSkills(resume, updateDto.SkillIds);

            await _context.SaveChangesAsync();
        }

        //Sirket hissəsində resumelerin metodlari
        public async Task<DataListDto<ResumeListDto>> GetAllResumesAsync(string? fullname, bool? isPublic, List<ProfessionDegree>? professionDegree, Citizenship? citizenship, Gender? gender, bool? isExperience, JobStatus? jobStatus, List<Guid>? skillIds, List<LanguageFilterDto>? languages, int skip, int take)
        {
            var query = _context.Resumes
                .Where(r => !r.IsAnonym)
                .Include(r => r.ResumeSkills)
                .ThenInclude(rs => rs.Skill.Translations)
                .Include(r => r.Languages)
                .Include(r => r.CompanyResumeAccesses)
                .AsNoTracking();

            query = ApplyFilters(query, fullname, isPublic, professionDegree, citizenship, gender, isExperience, skillIds, languages, jobStatus);

            var resumes = await query.Select(x => new ResumeListDto
            {
                Id = x.Id,
                FullName = x.IsPublic ? $"{x.FirstName} {x.LastName}" : x.CompanyResumeAccesses.Any(cra => cra.CompanyUserId == _currentUser.UserGuid && cra.ResumeId == x.Id) ? $"{x.FirstName} {x.LastName}" : null,
                ProfileImage = x.UserPhoto != null ? x.IsPublic ? $"{_currentUser.BaseUrl}/userFiles/{x.UserPhoto}" : x.CompanyResumeAccesses.Any(cra => cra.CompanyUserId == _currentUser.UserGuid && cra.ResumeId == x.Id) ? $"{_currentUser.BaseUrl}/userFiles/{x.UserPhoto}" : null : null,
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
                .Select(s => s.Skill.Translations.Where(x=> x.Language == _currentUser.LanguageCode).Select(z=> z.Name).FirstOrDefault())
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

        public async Task<DataListDto<ResumeListDto>> GetSavedResumesAsync(string? fullName, bool? isPublic, JobStatus? jobStatus, List<ProfessionDegree>? professionDegree, Citizenship? citizenship, Gender? gender, bool? isExperience, List<Guid>? skillIds, List<LanguageFilterDto>? languages, int skip, int take)
        {
            var resumeQuery = _context.SavedResumes
                .Where(sr => sr.CompanyUserId == (Guid)_currentUser.UserGuid!)
                .Include(sr => sr.Resume)
                    .ThenInclude(r => r.ResumeSkills)
                        .ThenInclude(rs => rs.Skill.Translations)
                .Include(sr => sr.Resume.CompanyResumeAccesses)
                .Select(sr => sr.Resume).AsQueryable()
                .AsNoTracking();

            resumeQuery = ApplyFilters(resumeQuery, fullName, isPublic, professionDegree, citizenship, gender, isExperience, skillIds, languages, jobStatus);

            var resumes = await resumeQuery
               .Select(x => new ResumeListDto
               {
                   Id = x.Id,
                   FullName = x.IsPublic ? $"{x.FirstName} {x.LastName}" : x.CompanyResumeAccesses.Any(cra => cra.CompanyUserId == _currentUser.UserGuid) ? $"{x.FirstName} {x.LastName}" : null,
                   ProfileImage = x.UserPhoto != null
                       ? x.IsPublic || x.CompanyResumeAccesses.Any(cra => cra.CompanyUserId == _currentUser.UserGuid)
                           ? $"{_currentUser.BaseUrl}/userFiles/{x.UserPhoto}"
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
                       .Select(s => s.Skill.Translations.Where(t=> t.Language == _currentUser.LanguageCode)
                       .Select(z=> z.Name)
                       .FirstOrDefault())
                       .ToList(),
                   Position = x.Position != null ? x.Position.Name : null,
                   HasAccess = x.IsPublic || x.CompanyResumeAccesses.Any(cra => cra.CompanyUserId == _currentUser.UserGuid)
               })
               .Skip((skip - 1) * take)
               .Take(take)
               .ToListAsync();

            return new DataListDto<ResumeListDto>
            {
                Datas = resumes,
                TotalCount = await resumeQuery.CountAsync()
            };
        }

        public async Task ToggleSaveResumeAsync(Guid resumeId)
        {
            if (!await _context.Resumes.AnyAsync(x => x.Id == resumeId))
                throw new NotFoundException();

            var existSaveResume = await _context.SavedResumes.FirstOrDefaultAsync(x => x.ResumeId == resumeId && x.CompanyUserId == _currentUser.UserGuid);

            if (existSaveResume == null)
            {
                await _context.SavedResumes.AddAsync(new SavedResume
                {
                    CompanyUserId = (Guid)_currentUser.UserGuid!,
                    ResumeId = resumeId,
                    SaveDate = DateTime.Now
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

        public async Task<ResumeDetailItemDto> GetByIdResumeAysnc(Guid resumeId)
        {
            var userId = _currentUser.UserGuid;

            var resumeData = await _context.Resumes
                .Where(r => r.Id == resumeId)
                    .Include(r => r.SavedResumes)
                    .Include(r => r.Position)
                    .Include(r => r.PhoneNumbers)
                    .Include(r => r.Educations)
                    .Include(r => r.Experiences)
                    .Include(r => r.Languages)
                    .Include(r => r.Certificates)
                .Select(r => new
                {
                    Resume = r,
                    HasAccessByCompany = r.CompanyResumeAccesses.Any(x => x.CompanyUserId == userId),
                })
                .FirstOrDefaultAsync() ?? throw new NotFoundException();

            bool hasApplied = false;

            // Əgər CompanyUser-dursa, şirkətin hər hansı bir vakansiyasına müraciət edib-etmədiyini yoxlayırıq
            //TODO : burada request responseden istifade edilmeli deyil
            if (!resumeData.HasAccessByCompany && (_currentUser.UserRole == (byte)UserRole.CompanyUser || _currentUser.UserRole == (byte)UserRole.EmployeeUser))
            {
                var checkApplicationResponse = await _checkApplicationRequest.GetResponse<CheckApplicationResponse>(
                    new CheckApplicationRequest
                    {
                        CompanyUserId = (Guid)userId,
                        UserId = resumeData.Resume.UserId
                    });

                hasApplied = checkApplicationResponse.Message.HasApplied;
            }

            // İcazə olub-olmadığını yoxlayırıq. 4 halda resume-nin bütün datalarına tam baxma icazəsi var:
            bool hasFullAccess = _currentUser.UserRole == (byte)UserRole.Admin // Əgər admindirsə
                || _currentUser.UserRole == (byte)UserRole.SuperAdmin          // Əgər Superadmindirsə
                || hasApplied                                                  // Əgər müraciət edibsə
                || resumeData.HasAccessByCompany                               // Əgər resume-yə baxma icazəsi varsa
                || resumeData.Resume.IsPublic;                                 // Əgər resume public-dirsə

            var resume = resumeData.Resume;

            var resumeResponse = new ResumeDetailItemDto
            {
                //ResumeId = resume.Id,
                //UserId = resume.UserId,
                IsSaved = resume.SavedResumes.Any(sr => sr.ResumeId == resume.Id && sr.CompanyUserId == _currentUser.UserGuid),
                FirstName = hasFullAccess ? resume.FirstName : null,
                LastName = hasFullAccess ? resume.LastName : null,
                ResumeEmail = hasFullAccess ? resume.ResumeEmail : null,

                PhoneNumbers = hasFullAccess
                    ? resume.PhoneNumbers.Select(p => new NumberGetByIdDto
                    {
                        PhoneNumberId = p.Id,
                        PhoneNumber = p.PhoneNumber
                    }).ToList()
                    : [],

                UserPhoto = hasFullAccess && resume.UserPhoto != null
                    ? $"{_currentUser.BaseUrl}/userFiles/{resume.UserPhoto}"
                    : null,

                BirthDay = resume.BirthDay,
                Gender = resume.Gender,
                IsMarried = resume.IsMarried,
                IsDriver = resume.IsDriver,
                IsCitizen = resume.IsCitizen,
                MilitarySituation = resume.MilitarySituation,
                Adress = resume.Adress,
                Position = resume.Position?.Name,
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
                    CertificateFile = $"{_currentUser.BaseUrl}/userFiles/{c.CertificateFile}"
                }).ToList(),
                HasAccess = hasFullAccess
            };

            resumeResponse.Skills = await _context.ResumeSkills.Where(x => x.ResumeId == resumeId)
            .Select(x => new SkillGetByIdDto
            {
                Id = x.SkillId,
                Name = x.Skill.Translations.Where(x => x.Language == _currentUser.LanguageCode).Select(x => x.Name).FirstOrDefault()
            }).ToListAsync();

            return resumeResponse;
        }

        public async Task TakeResumeAccessAsync(Guid resumeId)
        {
            if (await _context.CompanyResumeAccesses.AnyAsync(x => x.ResumeId == resumeId && x.CompanyUserId == _currentUser.UserGuid))
                throw new IsAlreadyExistException<CompanyResumeAccess>(MessageHelper.GetMessage("RESUME_ACCESS_ALREADY_EXIST"));

            var resume = await _context.Resumes.Where(x => x.Id == resumeId)
                .Select(x => new
                {
                    x.IsPublic
                }).FirstOrDefaultAsync();

            if (resume == null)
                throw new NotFoundException();

            if (resume.IsPublic) throw new ResumeIsPublicException();

            var checkBalanceResponse = await _balanceRequest.GetResponse<CheckBalanceResponse>(new CheckBalanceRequest
            {
                InformationType = InformationType.AnonymResume,
                UserId = (Guid)_currentUser.UserGuid
            });
            if (checkBalanceResponse.Message.HasEnoughBalance)
            {
                await _context.CompanyResumeAccesses.AddAsync(new CompanyResumeAccess
                {
                    AccessDate = DateTime.Now,
                    CompanyUserId = (Guid)_currentUser.UserGuid,
                    ResumeId = resumeId
                });

                await _context.SaveChangesAsync();

                //Odenisin olmasi ucun
                await _publishEndpoint.Publish(new PayEvent
                {
                    InformationType = InformationType.AnonymResume,
                    InformationId = resumeId,
                    UserId = (Guid)_currentUser.UserGuid
                });
            }
            else throw new BadRequestException(MessageHelper.GetMessage("INSUFFICIENT_BALANCE"));
        }

        #region Private Methods

        private IQueryable<Core.Entities.Resume> ApplyFilters(IQueryable<Core.Entities.Resume> query, string? fullname, bool? isPublic, List<ProfessionDegree>? professionDegrees, Citizenship? citizenship, Gender? gender, bool? isExperience, List<Guid>? skillIds, List<LanguageFilterDto>? languages, JobStatus? jobStatus)
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

            if (professionDegrees != null && professionDegrees.Any())
            {
                query = query.Where(x => x.Educations.Any(e => professionDegrees.Contains(e.ProfessionDegree)));
            }

            if (citizenship != null)
            {
                query = query.Where(x => x.IsCitizen == citizenship);
            }

            if (gender != null)
            {
                query = query.Where(x => x.Gender == gender);
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
                query = query.Where(x => x.ResumeSkills.Any(rs => skillIds.Contains(rs.SkillId)));
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

        private async Task<Core.Entities.Resume> BuildResumeAsync(ResumeCreateDto dto, Guid positionId)
        {
            FileDto fileResult = dto.UserPhoto != null
                ? await _fileService.UploadAsync(FilePaths.image, dto.UserPhoto)
                : new FileDto();

            string? email = dto.IsMainEmail
                ? (await _context.Users.Where(x=> x.Id == _currentUser.UserGuid).Select(x=> x.Email).FirstOrDefaultAsync())
                : dto.ResumeEmail;

            return MapToResumeEntity(dto, $"{fileResult.FilePath}/{fileResult.FileName}", email, positionId);
        }

        private Core.Entities.Resume MapToResumeEntity(ResumeCreateDto dto, string? filePath, string? email, Guid positionId)
        {
            return new Core.Entities.Resume
            {
                UserId = (Guid)_currentUser.UserGuid!,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
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

        private async Task<List<Core.Entities.Number>> GetPhoneNumbersAsync(ResumeCreateDto dto, Guid resumeId)
        {
            string? mainNumber = null;

            if (dto.IsMainNumber)
                mainNumber = (await _context.Users.Where(x=> x.Id == _currentUser.UserGuid).Select(x=> x.MainPhoneNumber).FirstOrDefaultAsync());

            return _numberService.CreateBulkNumber(dto.PhoneNumbers ?? [], resumeId, mainNumber);
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
                ?? throw new NotFoundException();
        }

        private static void UpdateResumePersonalInfo(Core.Entities.Resume resume, ResumeUpdateDto updateDto)
        {
            //resume.FatherName = updateDto.FatherName.Trim();
            resume.FirstName = updateDto.FirstName.Trim();
            resume.LastName = updateDto.LastName.Trim();
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

        private async Task UpdateUserPhotoAsync(Core.Entities.Resume resume, IFormFile userPhoto)
        {
            if (!string.IsNullOrEmpty(resume.UserPhoto))
            {
                _fileService.DeleteFile(resume.UserPhoto);
            }

            var fileResult = await _fileService.UploadAsync(FilePaths.image, userPhoto);
            resume.UserPhoto = $"{fileResult.FilePath}/{fileResult.FileName}";
        }

        private async Task UpdateResumeEmailAsync(Core.Entities.Resume resume, bool IsMainEmail, string? resumeEmail)
        {
            resume.ResumeEmail = IsMainEmail
                ? (await _context.Users.Where(x=> x.Id == _currentUser.UserGuid).Select(x=> x.Email).FirstOrDefaultAsync())
                : resumeEmail;
        }

        private async Task UpdatePhoneNumbersAsync(Core.Entities.Resume resume, ICollection<NumberUpdateDto> phoneNumbers, bool isMainNumber)
        {
            string? mainNumber = null;

            if (isMainNumber)
            {
                var userInfo = await _context.Users.Where(x=> x.Id == _currentUser.UserGuid).Select(x=> x.MainPhoneNumber).FirstOrDefaultAsync();
                mainNumber = userInfo;
            }

            resume.PhoneNumbers = await _numberService.UpdateBulkNumberAsync(phoneNumbers, resume.PhoneNumbers, resume.Id, mainNumber);
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