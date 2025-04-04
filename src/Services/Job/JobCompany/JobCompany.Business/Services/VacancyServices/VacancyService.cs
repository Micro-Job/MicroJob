using System.Data;
using System.Security.Claims;
using JobCompany.Business.Dtos.CategoryDtos;
using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.CompanyDtos;
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
        readonly IConfiguration _configuration;
        private readonly string? _authServiceBaseUrl;
        private readonly ICurrentUser _currentUser;
        private readonly IHttpContextAccessor _contextAccessor;

        public VacancyService(
            JobCompanyDbContext _context,
            IFileService fileService,
            IExamService examService,
            IPublishEndpoint publishEndpoint,
            IConfiguration configuration,
            ICurrentUser currentUser,
            IHttpContextAccessor contextAccessor
        )
        {
            this._context = _context;
            _fileService = fileService;
            _publishEndpoint = publishEndpoint;
            _configuration = configuration;
            _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
            _currentUser = currentUser;
            _contextAccessor = contextAccessor;
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

            if (vacancyDto.SkillIds != null)
            {
                await _publishEndpoint.Publish(
                    new VacancyCreatedEvent
                    {
                        SenderId = (Guid)_currentUser.UserGuid,
                        SkillIds = vacancyDto.SkillIds,
                        InformationId = vacancy.Id,
                        InformatioName = vacancy.Title,
                    }
                );
            }

            await _context.SaveChangesAsync();
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
        public async Task<List<VacancyGetAllDto>> GetAllOwnVacanciesAsync(string? titleName, string? categoryId, string? countryId, string? cityId, VacancyStatus? IsActive, decimal? minSalary, decimal? maxSalary, int skip = 1, int take = 6)
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
                null,
                null
            );

            var vacancies = await query
                .Select(x => new VacancyGetAllDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    StartDate = x.StartDate,
                    Location = x.Location,
                    CompanyLogo = $"{_authServiceBaseUrl}/{x.Company.CompanyLogo}",
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
        public async Task<DataListDto<VacancyGetByCompanyIdDto>> GetVacanciesByCompanyIdAsync(string companyId, int skip = 1, int take = 9)
        {
            var companyGuid = Guid.Parse(companyId);

            var query = _context.Vacancies.Where(x => x.CompanyId == companyGuid && x.VacancyStatus == VacancyStatus.Active && x.EndDate >= DateTime.Now).AsQueryable().AsNoTracking();

            var vacancies = await query
                .Select(x => new VacancyGetByCompanyIdDto
                {
                    VacancyId = x.Id,
                    CompanyName = x.CompanyName,
                    Title = x.Title,
                    Location = x.Location,
                    CompanyLogo = $"{_authServiceBaseUrl}/{x.Company.CompanyLogo}",
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

            Guid userGuid = (Guid)_currentUser.UserGuid;

            var vacancyDto = await _context
                    .Vacancies.Where(x => x.Id == vacancyGuid)
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
                                VacancyNumber = vn.Number,
                            })
                            .ToList(),
                        Skills = x
                            .VacancySkills.Where(vc => vc.Skill != null)
                            .Select(vc => new SkillDto { Name = vc.Skill.Translations.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name) })
                            .ToList(),
                        CompanyName = x.CompanyName,
                        CategoryName = x.Category.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name),
                        CompanyUserId = x.Company.UserId
                    })
                    .FirstOrDefaultAsync() ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));

            var existVacancy = await _context.Vacancies.FirstOrDefaultAsync(x => x.Id == vacancyGuid);

            if (vacancyDto.CompanyUserId != userGuid)
            {
                existVacancy.ViewCount++;
                await _context.SaveChangesAsync();
            }

            return vacancyDto;
        }

        /// <summary> vacancynin update olunması usere notification </summary>
        public async Task UpdateVacancyAsync(UpdateVacancyDto vacancyDto, ICollection<UpdateNumberDto>? numberDtos)
        {
            var vacancyGuid = Guid.Parse(vacancyDto.Id);
            var existingVacancy =
                await _context
                    .Vacancies.Where(v => v.Id == vacancyGuid && v.Company.UserId == _currentUser.UserGuid)
                    .FirstOrDefaultAsync() ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));

            if (existingVacancy.VacancyStatus == VacancyStatus.Block)
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
                foreach (var numberDto in numberDtos)
                {
                    var phoneNumberGuid = Guid.Parse(numberDto.Id);
                    var phoneNumber = existingVacancy.VacancyNumbers?.FirstOrDefault(p =>
                        p.Id == phoneNumberGuid
                    );
                    if (phoneNumber != null && phoneNumber.Number != numberDto.PhoneNumber)
                    {
                        phoneNumber.Number = numberDto.PhoneNumber;
                    }
                }
            }


            await _context.SaveChangesAsync();

            var userIds = await _context
                .Applications.Where(a => a.VacancyId == vacancyGuid && a.Status.StatusEnum != StatusEnum.Rejected)
                .Select(a => a.UserId)
                .ToListAsync();

            await _publishEndpoint.Publish(
                new VacancyUpdatedEvent
                {
                    InformationId = vacancyGuid,
                    SenderId = (Guid)_currentUser.UserGuid,
                    UserIds = userIds,
                    InformationName = existingVacancy.Title,
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
                    CompanyLogo = v.CompanyLogo != null ? $"{_authServiceBaseUrl}/{v.CompanyLogo}" : null,
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
                CompanyLogo = x.CompanyLogo != null ? $"{_authServiceBaseUrl}/{x.CompanyLogo}" : null,
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

        public async Task<DataListDto<VacancyGetAllDto>> GetAllSavedVacancyAsync(int skip, int take)
        {
            var query = _context.SavedVacancies.Where(x => x.UserId == _currentUser.UserGuid)
                                               .AsQueryable()
                                               .AsNoTracking();

            var vacancies = await query
            .Select(x => new VacancyGetAllDto
            {
                Id = x.Vacancy.Id,
                Title = x.Vacancy.Title,
                CompanyLogo = x.Vacancy.CompanyLogo != null ? $"{_authServiceBaseUrl}/{x.Vacancy.CompanyLogo}" : null,
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
    }
}
