using System.Data;
using System.Security.Claims;
using JobCompany.Business.Dtos.CategoryDtos;
using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.CompanyDtos;
using JobCompany.Business.Dtos.MessageDtos;
using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Dtos.SkillDtos;
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.Business.Exceptions.VacancyExceptions;
using JobCompany.Business.Extensions;
using JobCompany.Business.Services.ExamServices;
using JobCompany.Business.Statistics;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Dtos.VacancyDtos;
using Shared.Events;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.Enums;
using SharedLibrary.Events;
using SharedLibrary.Exceptions;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Statics;

namespace JobCompany.Business.Services.VacancyServices
{
    public class VacancyService : IVacancyService
    {
        private readonly JobCompanyDbContext _context;
        private readonly IFileService _fileService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ICurrentUser _currentUser;

        public VacancyService(JobCompanyDbContext context, IFileService fileService, IPublishEndpoint publishEndpoint, ICurrentUser currentUser)
        {
            _context = context;
            _fileService = fileService;
            _publishEndpoint = publishEndpoint;
            _currentUser = currentUser;
        }

        /// <summary> vacancy yaradılması </summary>
        /// vacancy yaradilan zaman exam yaradılması
        public async Task CreateVacancyAsync(CreateVacancyDto vacancyDto, ICollection<CreateNumberDto>? numberDto)
        {
            string? companyLogoPath = null;
            var company = await _context.Companies.Where(x => x.UserId == _currentUser.UserGuid).Select(x => new
            {
                x.Id,
                x.CompanyName,
                x.CompanyLogo
            }).FirstOrDefaultAsync();

            if (company != null && !string.IsNullOrEmpty(company.CompanyLogo))
            {
                companyLogoPath = company.CompanyLogo;
            }
            else if (vacancyDto.CompanyLogo != null)
            {
                FileDto fileResult = await _fileService.UploadAsync(FilePaths.image, vacancyDto.CompanyLogo);
                companyLogoPath = $"{fileResult.FilePath}/{fileResult.FileName}";
            }

            var vacancy = new Vacancy
            {
                Id = Guid.NewGuid(),
                CompanyName = _currentUser.UserRole == (byte)UserRole.CompanyUser ? company.CompanyName : vacancyDto.CompanyName.Trim(),
                CompanyId = company?.Id,
                Title = vacancyDto.Title.Trim(),
                CompanyLogo = companyLogoPath,
                StartDate = vacancyDto.StartDate,
                EndDate = vacancyDto.EndDate,
                CreatedDate = DateTime.Now,
                Location = vacancyDto.Location,
                CountryId = vacancyDto.CountryId,
                CityId = vacancyDto.CityId,
                Email = vacancyDto.Email,
                WorkType = vacancyDto.WorkType,
                WorkStyle = vacancyDto.WorkStyle,
                MainSalary = vacancyDto.MainSalary,
                MaxSalary = vacancyDto.MaxSalary,
                Requirement = vacancyDto.Requirement,
                Description = vacancyDto.Description,
                Gender = vacancyDto.Gender,
                Military = vacancyDto.Military,
                Driver = vacancyDto.Driver,
                Family = vacancyDto.Family,
                Citizenship = vacancyDto.Citizenship,
                CategoryId = vacancyDto.CategoryId,
                ExamId = vacancyDto.ExamId,
                VacancyStatus = VacancyStatus.Pending,
                ViewCount = 0
            };

            var numbers = new List<VacancyNumber>();
            if (numberDto != null)
            {
                numbers = numberDto
                    .Select(numberCreateDto => new VacancyNumber
                    {
                        Number = numberCreateDto.PhoneNumber,
                        VacancyId = vacancy.Id,
                    })
                    .ToList();

                await _context.VacancyNumbers.AddRangeAsync(numbers);
                vacancy.VacancyNumbers = numbers;
            }

            var vacancySkills =
                vacancyDto.SkillIds != null
                    ? vacancyDto
                        .SkillIds.Select(skillId => new VacancySkill
                        {
                            SkillId = skillId,
                            VacancyId = vacancy.Id,
                        })
                        .ToList()
                    : [];
            vacancy.VacancySkills = vacancySkills;

            await _context.Vacancies.AddAsync(vacancy);
            await _context.SaveChangesAsync();

            //if (vacancyDto.SkillIds != null)
            //{
            //    await _publishEndpoint.Publish(
            //        new VacancyCreatedEvent
            //        {
            //            SenderId = (Guid)_currentUser.UserGuid,
            //            SkillIds = vacancyDto.SkillIds,
            //            InformationId = vacancy.Id,
            //            InformatioName = vacancy.Title,
            //        }
            //    );
            //}
        }

        public async Task DeleteAsync(List<string> ids)
        {
            if (ids == null || ids.Count == 0)
                throw new ArgumentException(MessageHelper.GetMessage("NOT_EMPTY"));
            var vacancyGuids = ids.Select(id =>
                {
                    if (Guid.TryParse(id, out var guid))
                        return guid;
                    throw new FormatException(MessageHelper.GetMessage("INVALID_FORMAT"));
                })
                .ToList();

            var deletedCount = await _context
                .Vacancies.Where(x => vacancyGuids.Contains(x.Id) && x.Company.UserId == _currentUser.UserGuid)
                .ExecuteUpdateAsync(x => x.SetProperty(v => v.VacancyStatus, VacancyStatus.Deactive));

            if (deletedCount == 0)
                throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));
        }

        /// <summary> Şirkətin profilində bütün vakansiyalarını gətirmək(Filterlerle birlikde) </summary>
        public async Task<List<VacancyGetAllDto>> GetAllOwnVacanciesAsync(string? titleName, string? categoryId, string? countryId, string? cityId, VacancyStatus? IsActive, decimal? minSalary, decimal? maxSalary, byte? workStyle, byte? workType, int skip = 1, int take = 6)
        {
            var query = _context
                .Vacancies.Where(x => x.Company.UserId == _currentUser.UserGuid)
                .AsNoTracking();

            query = ApplyVacancyFilters(
                query,
                titleName,
                categoryId,
                countryId,
                cityId,
                IsActive,
                minSalary,
                maxSalary,
                null,
                workStyle,
                workType
            );

            var vacancies = await query
                .Select(x => new VacancyGetAllDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    StartDate = x.StartDate,
                    Location = x.Location,
                    CompanyLogo = $"{_currentUser.BaseUrl}/{x.Company.CompanyLogo}",
                    CompanyName = x.CompanyName,
                    ViewCount = x.ViewCount,
                    WorkType = x.WorkType,
                    WorkStyle = x.WorkStyle,
                    MainSalary = x.MainSalary,
                    MaxSalary = x.MaxSalary,
                    VacancyStatus = x.VacancyStatus
                })
                .Skip(Math.Max(0, (skip - 1) * take))
                .Take(take)
                .ToListAsync();

            return vacancies;
        }

        /// <summary> ??? </summary>
        public async Task<List<VacancyListDtoForAppDto>> GetAllVacanciesForAppAsync()
        {
            var vacancies = await _context
                .Vacancies.Where(x => x.Company.UserId == _currentUser.UserGuid && x.VacancyStatus == VacancyStatus.Active)
                .Select(x => new VacancyListDtoForAppDto
                {
                    VacancyId = x.Id,
                    VacancyName = x.Title,
                })
                .ToListAsync();

            return vacancies;
        }

        /// <summary> şirkət id'sinə görə vacanciyaların gətirilməsi </summary>
        public async Task<DataListDto<VacancyGetByCompanyIdDto>> GetVacanciesByCompanyIdAsync(string companyId, Guid? vacancyId , int skip = 1, int take = 9)
        {
            var companyGuid = Guid.Parse(companyId);

            var query = _context.Vacancies
                .Where(x => x.CompanyId == companyGuid && 
                            x.VacancyStatus == VacancyStatus.Active && 
                            x.EndDate >= DateTime.Now)
                .AsQueryable()
                .AsNoTracking();

            if (vacancyId != null)
                query = query.Where(x=> x.Id != vacancyId);

            var vacancies = await query
                .Select(x => new VacancyGetByCompanyIdDto
                {
                    VacancyId = x.Id,
                    CompanyName = x.CompanyName,
                    Title = x.Title,
                    Location = x.Location,
                    CompanyLogo = $"{_currentUser.BaseUrl}/{x.Company.CompanyLogo}",
                    WorkStyle = x.WorkStyle,
                    WorkType = x.WorkType,
                    StartDate = x.StartDate,
                    ViewCount = x.ViewCount,
                    MainSalary = x.MainSalary,
                    MaxSalary = x.MaxSalary,
                })
                .Skip(Math.Max(0, (skip - 1) * take))
                .Take(take)
                .ToListAsync();

            return new DataListDto<VacancyGetByCompanyIdDto>
            {
                Datas = vacancies,
                TotalCount = await query.CountAsync()
            };
        }

        /// <summary> vacanciya id'sinə görə vacancyın gətirilməsi </summary>
        public async Task<VacancyGetByIdDto> GetByIdVacancyAsync(string id)
        {
            var vacancyGuid = Guid.Parse(id);
                
            Guid? userGuid = _currentUser.UserGuid;

            var vacancyDto = await _context.Vacancies
                .Where(x => x.Id == vacancyGuid)
                    .Include(x => x.Category)
                        .ThenInclude(x => x.Translations)
                    .Include(x => x.VacancyMessages)
                        .ThenInclude(c => c.Message)
                            .ThenInclude(x => x.Translations)
                    .Select(x => new VacancyGetByIdDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        CompanyId = x.CompanyId,
                        CompanyLogo = $"{_currentUser.BaseUrl}/{x.Company.CompanyLogo}",
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Location = x.Location,
                        ViewCount = x.ViewCount,
                        WorkType = x.WorkType,
                        WorkStyle = x.WorkStyle,
                        MainSalary = x.MainSalary,
                        MaxSalary = x.MaxSalary,
                        Requirement = x.Requirement,
                        Description = x.Description,
                        Email = x.Email,
                        Gender = x.Gender,
                        Military = x.Military,
                        Family = x.Family,
                        Driver = x.Driver,
                        Citizenship = x.Citizenship,
                        ExamId = x.ExamId,
                        IsSaved = userGuid != null ? x.SavedVacancies.Any(y => y.UserId == userGuid && y.VacancyId == vacancyGuid) : false,
                        VacancyNumbers = x
                            .VacancyNumbers.Select(vn => new VacancyNumberDto
                            {
                                Id = vn.Id,
                                VacancyNumber = vn.Number,
                            })
                            .ToList(),
                        //Skills = x.VacancySkills
                        //        .Where(vc => vc.Skill != null)
                        //        .Select(vc => new SkillDto
                        //        {
                        //            Id = vc.Skill.Id,
                        //            Name = vc.Skill.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name)
                        //        }).ToList(),
                        CompanyName = x.CompanyName,
                        CategoryName = x.Category.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name),
                        CompanyUserId = x.Company.UserId,
                        Messages = _currentUser.UserGuid == x.Company.UserId
                            ? x.VacancyMessages.Select(vm => vm.Message.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Content)).ToList()
                            : null
                    })
                    .FirstOrDefaultAsync() ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));


            if (vacancyDto.CompanyUserId != userGuid)
            {
                var existVacancy = await _context.Vacancies.FirstOrDefaultAsync(x => x.Id == vacancyGuid);
                existVacancy.ViewCount++;
                await _context.SaveChangesAsync();
            }

            return vacancyDto;
        }

        /// <summary> Vakansiyanın bütün detallarını gətirir </summary>
        public async Task<VacancyDetailsDto> GetVacancyDetailsAsync(Guid id)
        {
            var vacancy = await _context.Vacancies
                .Where(x => x.Id == id && x.Company.UserId == _currentUser.UserGuid)
                .Include(x => x.Country).ThenInclude(c => c.Translations)
                .Include(x => x.City).ThenInclude(c => c.Translations)
                .Include(x => x.Category).ThenInclude(c => c.Translations)
                .Include(v => v.VacancySkills)
                            .ThenInclude(vs => vs.Skill)
                                .ThenInclude(s => s.Translations)
                .Select(v => new VacancyDetailsDto
                {
                    Id = v.Id,
                    Title = v.Title,
                    CompanyId = v.CompanyId,
                    CompanyName = v.CompanyName,
                    CompanyUserId = v.Company.UserId,
                    CompanyLogo = $"{_currentUser.BaseUrl}/{v.Company.CompanyLogo}",
                    StartDate = v.StartDate,
                    EndDate = v.EndDate,
                    Location = v.Location,
                    ViewCount = v.ViewCount,
                    WorkType = v.WorkType,
                    WorkStyle = v.WorkStyle,
                    MainSalary = v.MainSalary,
                    MaxSalary = v.MaxSalary,
                    Requirement = v.Requirement,
                    Description = v.Description,
                    Email = v.Email,
                    Gender = v.Gender,
                    Military = v.Military,
                    Family = v.Family,
                    Driver = v.Driver,
                    Citizenship = v.Citizenship,
                    ExamId = v.ExamId,
                    CreatedDate = v.CreatedDate,
                    VacancyStatus = v.VacancyStatus,
                    CategoryId = v.CategoryId,
                    CategoryName = v.Category != null ? v.Category.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name) : null,
                    CityId = v.CityId,
                    CityName = v.City != null ? v.City.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name) : null,
                    CountryId = v.CountryId,
                    CountryName = v.Country != null ? v.Country.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name) : null,

                    VacancyNumbers = v.VacancyNumbers != null ? v
                                .VacancyNumbers.Select(vn => new VacancyNumberDto
                                {
                                    Id = vn.Id,
                                    VacancyNumber = vn.Number,
                                }).ToList() : new List<VacancyNumberDto>(),

                    Skills = v.VacancySkills
                                .Where(vc => vc.Skill != null)
                                .Select(vc => new SkillDto
                                {
                                    Id = vc.Skill.Id,
                                    Name = vc.Skill.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name)
                                }).ToList()
                }).FirstOrDefaultAsync() ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));

            return vacancy;
        }


        /// <summary> vacancynin update olunması usere notification </summary>
        public async Task UpdateVacancyAsync(UpdateVacancyDto vacancyDto, ICollection<UpdateNumberDto>? numberDtos)
        {
            var vacancyGuid = Guid.Parse(vacancyDto.Id);
            var existingVacancy =
                await _context
                    .Vacancies.Where(v => v.Id == vacancyGuid && v.Company.UserId == _currentUser.UserGuid)
                    .Include(v => v.VacancySkills)
                    .Include(v => v.VacancyNumbers)
                    .FirstOrDefaultAsync() ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));

            if (existingVacancy.VacancyStatus == VacancyStatus.Block && existingVacancy.VacancyStatus == VacancyStatus.Reject)
                throw new VacancyUpdateException(MessageHelper.GetMessage("VACANCY_UPDATE"));

            existingVacancy.CompanyId = Guid.Parse(vacancyDto.CompanyId);
            existingVacancy.CompanyName = vacancyDto.CompanyName;
            existingVacancy.Title = vacancyDto.Title;
            existingVacancy.StartDate = vacancyDto.StartDate;
            existingVacancy.EndDate = vacancyDto.EndDate;
            existingVacancy.Location = vacancyDto.Location;
            existingVacancy.CountryId = Guid.Parse(
                vacancyDto.CountryId ?? throw new Exception(MessageHelper.GetMessage("NOT_FOUND"))
            );
            existingVacancy.CityId = Guid.Parse(vacancyDto.CityId ?? throw new Exception(MessageHelper.GetMessage("NOT_FOUND")));
            existingVacancy.Email = vacancyDto.Email;
            existingVacancy.ExamId = vacancyDto.ExamId;
            existingVacancy.WorkType = vacancyDto.WorkType;
            existingVacancy.WorkStyle = vacancyDto.WorkStyle;
            existingVacancy.MainSalary = vacancyDto.MainSalary;
            existingVacancy.MaxSalary = vacancyDto.MaxSalary;
            existingVacancy.Requirement = vacancyDto.Requirement;
            existingVacancy.Description = vacancyDto.Description;
            existingVacancy.Gender = vacancyDto.Gender;
            existingVacancy.Military = vacancyDto.Military;
            existingVacancy.Driver = vacancyDto.Driver;
            existingVacancy.Family = vacancyDto.Family;
            existingVacancy.Citizenship = vacancyDto.Citizenship;
            existingVacancy.VacancyStatus = VacancyStatus.Update;
            existingVacancy.CategoryId = Guid.Parse(
                vacancyDto.CategoryId ?? throw new Exception(MessageHelper.GetMessage("NOT_FOUND"))
            );

            if (numberDtos is not null)
            {
                var existingNumbers = existingVacancy.VacancyNumbers ?? [];
                var existingNumberDict = existingNumbers.ToDictionary(n => n.Id, n => n);

                var incomingNumberDict = numberDtos
                    .Where(n => !string.IsNullOrWhiteSpace(n.Id))
                    .ToDictionary(n => Guid.Parse(n.Id!), n => n.PhoneNumber);

                // Id-si olan nömrələri güncəlləyirik
                foreach (var kvp in incomingNumberDict)
                {
                    if (existingNumberDict.TryGetValue(kvp.Key, out var existingNumber))
                    {
                        if (existingNumber.Number != kvp.Value)
                            existingNumber.Number = kvp.Value;
                    }
                }

                // Müqayisə edib silinəcək nömrələri tapırıq
                var toRemove = existingNumbers
                    .Where(n => !incomingNumberDict.ContainsKey(n.Id))
                    .ToList();

                if (toRemove.Count != 0)
                {
                    _context.VacancyNumbers.RemoveRange(toRemove);
                }

                // Id-si olmayan yeni nömrələri əlavə et
                var newNumbers = numberDtos
                    .Where(n => string.IsNullOrWhiteSpace(n.Id))
                    .Select(n => new VacancyNumber
                    {
                        Number = n.PhoneNumber,
                        VacancyId = existingVacancy.Id
                    }).ToList();

                if (newNumbers.Count != 0)
                {
                    await _context.VacancyNumbers.AddRangeAsync(newNumbers);
                }
            }



            if (vacancyDto.Skills is not null)
            {
                var existingSkillIds = existingVacancy.VacancySkills.Select(vs => vs.SkillId).ToList(); // Mövcud olan skill id-ləri
                var incomingSkillIds = vacancyDto.Skills.Select(s => s.Id).ToList(); // Request-də gələn skill id-ləri

                var skillsToAdd = incomingSkillIds.Except(existingSkillIds).ToList(); // Müqayisə edib əlavə olunacaq skill id-ləri tapırıq

                var newVacancySkills = skillsToAdd.Select(skillId => new VacancySkill
                {
                    VacancyId = existingVacancy.Id,
                    SkillId = skillId
                }).ToList();

                if (newVacancySkills.Count != 0)
                {
                    await _context.VacancySkills.AddRangeAsync(newVacancySkills);  // Yeni skilllər əlavə olunur
                }


                var skillsToRemove = existingSkillIds.Except(incomingSkillIds).ToList(); // Müqayisə edib silinəcək skill id-ləri tapırıq
                if (skillsToRemove.Count != 0)
                {
                    var vacancySkillsToRemove = existingVacancy.VacancySkills
                        .Where(vs => skillsToRemove.Contains(vs.SkillId))
                        .ToList();

                    _context.VacancySkills.RemoveRange(vacancySkillsToRemove);  // skillər silinir
                }
            }


            await _context.SaveChangesAsync();

            var userIds = await _context
                .Applications.Where(a => a.VacancyId == vacancyGuid && a.Status.StatusEnum != StatusEnum.Rejected)
                .Select(a => a.UserId)
                .ToListAsync();

            await _publishEndpoint.Publish(  //Vakansiya update olunduqda müraciət edən userlərə notification göndərilir
                new NotificationToUserEvent
                {
                    InformationId = vacancyGuid,
                    SenderId = (Guid)_currentUser.UserGuid,
                    ReceiverIds = userIds,
                    InformationName = existingVacancy.Title,
                    NotificationType = NotificationType.VacancyUpdate,
                    SenderName = existingVacancy.CompanyName,
                    SenderImage = $"{_currentUser.BaseUrl}/{existingVacancy.CompanyLogo}",
                }
            );
        }

        /// <summary> Şirkət profilində vakansiya axtarışı vakansiya filterlere görə </summary>
        public async Task<DataListDto<VacancyGetAllDto>> GetAllVacanciesAsync(string? titleName, string? categoryId, string? countryId, string? cityId, decimal? minSalary, decimal? maxSalary, string? companyId, byte? workStyle, byte? workType, int skip = 1, int take = 9)
        {
            var query = _context.Vacancies.Where(x => x.VacancyStatus == VacancyStatus.Active && x.EndDate > DateTime.Now)
                .AsNoTracking()
                .AsQueryable();

            query = ApplyVacancyFilters(query, titleName, categoryId, countryId, cityId, null, minSalary, maxSalary, companyId, workStyle, workType);

            var vacancies = await query
                .Select(v => new VacancyGetAllDto
                {
                    Id = v.Id,
                    Title = v.Title,
                    CompanyLogo = v.Company.CompanyLogo != null ? $"{_currentUser.BaseUrl}/{v.Company.CompanyLogo}" : null,
                    CompanyName = v.CompanyName,
                    StartDate = v.StartDate,
                    Location = v.Location,
                    ViewCount = v.ViewCount,
                    WorkType = v.WorkType,
                    WorkStyle = v.WorkStyle,
                    MainSalary = v.MainSalary,
                    MaxSalary = v.MaxSalary,
                    IsSaved = _currentUser.UserId != null ? v.SavedVacancies.Any(x => x.VacancyId == v.Id && x.UserId == _currentUser.UserGuid) : false
                })
                .Skip(Math.Max(0, (skip - 1) * take))
                .Take(take)
                .ToListAsync();

            return new DataListDto<VacancyGetAllDto> { Datas = vacancies, TotalCount = await query.CountAsync() };
        }

        private static IQueryable<Vacancy> ApplyVacancyFilters(IQueryable<Vacancy> query, string? titleName, string? categoryId, string? countryId, string? cityId, VacancyStatus? isActive, decimal? minSalary, decimal? maxSalary, string? companyId, byte? workStyle, byte? workType)
        {
            if (titleName != null)
            {
                titleName = titleName.Trim();
                query = query.Where(x => x.Title.Contains(titleName));
            }

            if (isActive != null)
                query = query.Where(x => x.VacancyStatus == isActive);

            if (minSalary != null && maxSalary != null)
                query = query.Where(x => x.MainSalary >= minSalary && x.MaxSalary <= maxSalary);

            if (workType != null)
                query = query.Where(x => x.WorkType == (WorkType)workType);

            if (workStyle != null)
                query = query.Where(x => x.WorkStyle == (WorkStyle)workStyle);

            if (companyId != null)
            {
                Guid companyGuid = Guid.Parse(companyId);
                query = query.Where(x => x.CompanyId == companyGuid);
            }

            if (categoryId != null)
            {
                var categoryGuid = Guid.Parse(categoryId);
                query = query.Where(x => x.CategoryId == categoryGuid);
            }

            if (countryId != null)
            {
                var countryGuid = Guid.Parse(countryId);
                query = query.Where(x => x.CountryId == countryGuid);
            }

            if (cityId != null)
            {
                var cityGuid = Guid.Parse(cityId);
                query = query.Where(x => x.CityId == cityGuid);
            }

            return query;
        }

        public async Task ToggleSaveVacancyAsync(string vacancyId)
        {
            Guid vacancyGuid = Guid.Parse(vacancyId);

            if (!await _context.Vacancies.AnyAsync(x => x.Id == vacancyGuid))
                throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));

            var vacancyCheck = await _context.SavedVacancies.FirstOrDefaultAsync(x => x.VacancyId == vacancyGuid);

            if (vacancyCheck != null)
            {
                _context.SavedVacancies.Remove(vacancyCheck);
            }
            else
            {
                await _context.SavedVacancies.AddAsync(
                    new SavedVacancy { UserId = _currentUser.UserGuid, VacancyId = vacancyGuid }
                );
            }
            await _context.SaveChangesAsync();
        }

        public async Task TogglePauseVacancyAsync(Guid vacancyId)
        {
            var vacancy = await _context.Vacancies.FirstOrDefaultAsync(x => x.Id == vacancyId)
                ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));

            if (vacancy.VacancyStatus == VacancyStatus.Active)
            {
                vacancy.VacancyStatus = VacancyStatus.Pause;
            }
            else if (vacancy.VacancyStatus == VacancyStatus.Pause)
            {
                vacancy.VacancyStatus = VacancyStatus.Active;
            }
            else
                throw new VacancyStatusNotToggableException(MessageHelper.GetMessage("VACANCY_STATUS_NOT_TOGGABLE"));

            await _context.SaveChangesAsync();
        }

        public async Task<List<VacancyGetAllDto>> SimilarVacanciesAsync(string vacancyId, int take = 8)
        {
            var mainVacancy = await _context.Vacancies.Where(x => x.Id == Guid.Parse(vacancyId) && x.EndDate > DateTime.Now)
            .Select(x => new
            {
                x.Id,
                x.CategoryId
            }).FirstOrDefaultAsync();

            var vacancies = await _context.Vacancies
            .Where(x => x.CategoryId == mainVacancy.CategoryId && x.Id != mainVacancy.Id && x.VacancyStatus == VacancyStatus.Active && x.EndDate > DateTime.Now)
            .Select(x => new VacancyGetAllDto
            {
                Id = x.Id,
                Title = x.Title,
                CompanyLogo = x.CompanyLogo != null ? $"{_currentUser.BaseUrl}/{x.CompanyLogo}" : null,
                CompanyName = x.CompanyName,
                StartDate = x.StartDate,
                Location = x.Location,
                ViewCount = x.ViewCount,
                WorkType = x.WorkType,
                WorkStyle = x.WorkStyle,
                MainSalary = x.MainSalary,
                MaxSalary = x.MaxSalary,
                IsSaved = x.SavedVacancies.Any(y => y.VacancyId == x.Id && y.UserId == _currentUser.UserGuid)
            })
            .OrderByDescending(x => x.StartDate)
            .Take(take)
            .ToListAsync();

            return vacancies;
        }

        public async Task<DataListDto<VacancyGetAllDto>> GetAllSavedVacancyAsync(int skip, int take, string? vacancyName)
        {
            var query = _context.SavedVacancies.Where(x => x.UserId == _currentUser.UserGuid)
                                               .AsQueryable()
                                               .AsNoTracking();

            if (!string.IsNullOrEmpty(vacancyName))
            {
                vacancyName = vacancyName.Trim();
                query = query.Where(x => x.Vacancy.Title.ToLower().Contains(vacancyName.ToLower()));
            }

            var vacancies = await query
            .Select(x => new VacancyGetAllDto
            {
                Id = x.Vacancy.Id,
                Title = x.Vacancy.Title,
                CompanyLogo = x.Vacancy.Company.CompanyLogo != null ? $"{_currentUser.BaseUrl}/{x.Vacancy.Company.CompanyLogo}" : null,
                CompanyName = x.Vacancy.CompanyName,
                StartDate = x.Vacancy.StartDate,
                Location = x.Vacancy.Location,
                ViewCount = x.Vacancy.ViewCount,
                WorkType = x.Vacancy.WorkType,
                WorkStyle = x.Vacancy.WorkStyle,
                MainSalary = x.Vacancy.MainSalary,
                MaxSalary = x.Vacancy.MaxSalary,
                IsSaved = true
            })
            .Skip(Math.Max(0, (skip - 1) * take))
            .Take(take)
            .ToListAsync();

            return new DataListDto<VacancyGetAllDto>
            {
                Datas = vacancies,
                TotalCount = await query.CountAsync()
            };
        }

        /// <summary>
        /// Vakansiyanın şirkət tərəfindən ləğv edilməsi
        /// </summary>
        /// <param name="vacancyId"></param>
        /// <returns></returns>
        public async Task DeleteVacancyAsync(Guid vacancyId)
        {
            var vacancy = await _context.Vacancies.Include(x => x.Applications).FirstOrDefaultAsync(x => x.Id == vacancyId)
                ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));

            vacancy.PaymentDate = null;
            vacancy.VacancyStatus = VacancyStatus.Deleted;

            if (vacancy.Applications != null)
            {
                await _context.Applications.Where(x=> vacancy.Applications.Select(a=>a.Id).Contains(x.Id))
                    .ExecuteUpdateAsync(setter=> setter
                                                        .SetProperty(a=> a.IsDeleted , true)
                                                        .SetProperty(a=> a.IsActive , false));
            }
            await _context.SaveChangesAsync();
        }
    }
}
