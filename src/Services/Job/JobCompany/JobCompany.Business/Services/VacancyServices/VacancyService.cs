﻿using System.Security.Claims;
using JobCompany.Business.Dtos.CategoryDtos;
using JobCompany.Business.Dtos.CompanyDtos;
using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Dtos.SkillDtos;
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.Business.Services.ExamServices;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Events;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.Events;
using SharedLibrary.Exceptions;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Statics;

namespace JobCompany.Business.Services.VacancyServices
{
    public class VacancyService : IVacancyService
    {
        private readonly JobCompanyDbContext _context;
        private readonly Guid _userGuid;
        private readonly IFileService _fileService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IExamService _examService;
        private readonly IPublishEndpoint _publishEndpoint;
        readonly IConfiguration _configuration;
        private readonly string? _authServiceBaseUrl;

        public VacancyService(
            JobCompanyDbContext context,
            IFileService fileService,
            IHttpContextAccessor contextAccessor,
            IExamService examService,
            IPublishEndpoint publishEndpoint,
            IConfiguration configuration
        )
        {
            _context = context;
            _fileService = fileService;
            _contextAccessor = contextAccessor;
            _userGuid = Guid.Parse(
                _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value
                    ?? throw new Exception("Istifadeci sisteme daxil olmayib.")
            );
            _examService = examService;
            _publishEndpoint = publishEndpoint;
            _configuration = configuration;
            _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
        }

        /// <summary> vacancy yaradılması </summary>
        /// vacancy yaradilan zaman exam yaradılması
        public async Task CreateVacancyAsync(
            CreateVacancyDto vacancyDto,
            ICollection<CreateNumberDto>? numberDto
        )
        {
            string? companyLogoPath = null;
            var company = await _context.Companies.FirstOrDefaultAsync(x => x.UserId == _userGuid);

            if (company != null && !string.IsNullOrEmpty(company.CompanyLogo))
            {
                companyLogoPath = company.CompanyLogo;
            }
            else if (vacancyDto.CompanyLogo != null)
            {
                FileDto fileResult = await _fileService.UploadAsync(
                    FilePaths.image,
                    vacancyDto.CompanyLogo
                );
                companyLogoPath = $"{fileResult.FilePath}/{fileResult.FileName}";
            }

            var vacancy = new Vacancy
            {
                Id = Guid.NewGuid(),
                CompanyName = company.CompanyName,
                CompanyId = company?.Id,
                Title = vacancyDto.Title.Trim(),
                CompanyLogo = companyLogoPath,
                StartDate = vacancyDto.StartDate,
                EndDate = vacancyDto.EndDate,
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
                IsActive = true,
                ViewCount = 0,
            };

            // ?? TODO : burada eger numbers yoxdursa bos yere numbers AddRange olunur bunu ifin icinde etmek lazimdir bu vacancyNumberse de aiddir ??
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
                        SenderId = _userGuid,
                        SkillIds = vacancyDto.SkillIds,
                        InformationId = vacancy.Id,
                        Content =
                            $"Sizin resume skillərinizə uyğun yeni vakansiya yaradıldı: {vacancy.Title}",
                    }
                );
            }

            if (vacancyDto.Exam is not null)
            {
               var examId = await _examService.CreateExamAsync(vacancyDto.Exam);

                vacancy.ExamId = examId;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(List<string> ids)
        {
            if (ids == null || ids.Count == 0)
                throw new ArgumentException("ID list does not empty.");
            var vacancyGuids = ids.Select(id =>
                {
                    if (Guid.TryParse(id, out var guid))
                        return guid;
                    throw new FormatException($"Invalid ID format: {id}");
                })
                .ToList();

            var deletedCount = await _context
                .Vacancies.Where(x => vacancyGuids.Contains(x.Id) && x.Company.UserId == _userGuid)
                .ExecuteUpdateAsync(x => x.SetProperty(v => v.IsActive, false));

            if (deletedCount == 0)
                throw new NotFoundException<Vacancy>();
        }

        /// <summary> Şirkətin profilində bütün vakansiyalarını gətirmək(Filterlerle birlikde) </summary>
        public async Task<List<VacancyGetAllDto>> GetAllOwnVacanciesAsync(
            string? titleName,
            string? categoryId,
            string? countryId,
            string? cityId,
            bool? IsActive,
            decimal? minSalary,
            decimal? maxSalary,
            int skip = 1,
            int take = 6
        )
        {
            var query = _context
                .Vacancies.Where(x => x.Company.UserId == _userGuid)
                .Include(x => x.Company)
                .AsNoTracking();

            query = ApplyVacancyFilters(
                query,
                titleName,
                categoryId,
                countryId,
                cityId,
                IsActive,
                minSalary,
                maxSalary
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
                    IsActive = x.IsActive,
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
                .Vacancies.Where(x => x.Company.UserId == _userGuid && x.IsActive == true)
                .Select(x => new VacancyListDtoForAppDto
                {
                    VacancyId = x.Id,
                    VacancyName = x.Title,
                })
                .ToListAsync();

            return vacancies;
        }

        /// <summary> şirkət id'sinə görə vacanciyaların gətirilməsi </summary>
        public async Task<ICollection<VacancyGetByCompanyIdDto>> GetVacancyByCompanyIdAsync(
            string companyId,
            int skip = 1,
            int take = 9
        )
        {
            var companyGuid = Guid.Parse(companyId);

            var isCompanyExist = await _context.Companies.AnyAsync(x => x.Id == companyGuid);

            if (!isCompanyExist)
                throw new NotFoundException<Company>();

            var vacancies = await _context
                .Vacancies.Where(x => x.CompanyId == companyGuid && x.IsActive)
                .Include(x => x.Company)
                .Select(x => new VacancyGetByCompanyIdDto
                {
                    CompanyName = x.CompanyName,
                    Title = x.Title,
                    Location = x.Location,
                    CompanyLogo = $"{_authServiceBaseUrl}/{x.Company.CompanyLogo}",
                    WorkType = x.WorkType,
                    StartDate = x.StartDate,
                    ViewCount = x.ViewCount,
                    MainSalary = x.MainSalary,
                    MaxSalary = x.MaxSalary,
                })
                .Skip(Math.Max(0, (skip - 1) * take))
                .Take(take)
                .ToListAsync();

            return vacancies;
        }

        /// <summary> vacanciya id'sinə görə vacancyın gətirilməsi </summary>
        public async Task<VacancyGetByIdDto> GetByIdVacancyAsync(string id)
        {
            var vacancyGuid = Guid.Parse(id);
            var vacancy =
                await _context
                    .Vacancies.AsNoTracking()
                    .Where(x => x.Id == vacancyGuid)
                    .FirstOrDefaultAsync() ?? throw new NotFoundException<Vacancy>();

            var vacancyDto =
                await _context
                    .Vacancies.Where(x => x.Id == vacancyGuid)
                    .Select(x => new VacancyGetByIdDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        CompanyLogo = $"{_authServiceBaseUrl}/{x.Company.CompanyLogo}",
                        StartDate = x.StartDate,
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
                        VacancyNumbers = x
                            .VacancyNumbers.Select(vn => new VacancyNumberDto
                            {
                                VacancyNumber = vn.Number,
                            })
                            .ToList(),
                        Skills = x
                            .VacancySkills.Where(vc => vc.Skill != null)
                            .Select(vc => new SkillDto { Name = vc.Skill.Name })
                            .ToList(),
                        CompanyName = x.CompanyName,
                        CategoryName = x.Category.CategoryName,
                    })
                    .FirstOrDefaultAsync() ?? throw new NotFoundException<Vacancy>();

            return vacancyDto;
        }

        /// <summary> vacancynin update olunması usere notification </summary>
        public async Task UpdateVacancyAsync(
            UpdateVacancyDto vacancyDto,
            ICollection<UpdateNumberDto>? numberDtos
        )
        {
            var vacancyGuid = Guid.Parse(vacancyDto.Id);
            var existingVacancy =
                await _context
                    .Vacancies.Where(v => v.Id == vacancyGuid && v.Company.UserId == _userGuid)
                    .Select(v => new { Vacancy = v, CompanyId = v.Company.Id })
                    .FirstOrDefaultAsync() ?? throw new NotFoundException<Vacancy>();

            existingVacancy.Vacancy.CompanyId = Guid.Parse(vacancyDto.CompanyId);
            existingVacancy.Vacancy.CompanyName = vacancyDto.CompanyName;
            existingVacancy.Vacancy.Title = vacancyDto.Title;
            existingVacancy.Vacancy.StartDate = vacancyDto.StartDate;
            existingVacancy.Vacancy.EndDate = vacancyDto.EndDate;
            existingVacancy.Vacancy.Location = vacancyDto.Location;
            existingVacancy.Vacancy.CountryId = Guid.Parse(
                vacancyDto.CountryId ?? throw new Exception()
            );
            existingVacancy.Vacancy.CityId = Guid.Parse(vacancyDto.CityId ?? throw new Exception());
            existingVacancy.Vacancy.Email = vacancyDto.Email;
            existingVacancy.Vacancy.WorkType = vacancyDto.WorkType;
            existingVacancy.Vacancy.WorkStyle = vacancyDto.WorkStyle;
            existingVacancy.Vacancy.MainSalary = vacancyDto.MainSalary;
            existingVacancy.Vacancy.MaxSalary = vacancyDto.MaxSalary;
            existingVacancy.Vacancy.Requirement = vacancyDto.Requirement;
            existingVacancy.Vacancy.Description = vacancyDto.Description;
            existingVacancy.Vacancy.Gender = vacancyDto.Gender;
            existingVacancy.Vacancy.Military = vacancyDto.Military;
            existingVacancy.Vacancy.Driver = vacancyDto.Driver;
            existingVacancy.Vacancy.Family = vacancyDto.Family;
            existingVacancy.Vacancy.Citizenship = vacancyDto.Citizenship;
            existingVacancy.Vacancy.CategoryId = Guid.Parse(
                vacancyDto.CategoryId ?? throw new Exception()
            );

            if (numberDtos is not null)
            {
                foreach (var numberDto in numberDtos)
                {
                    var phoneNumberGuid = Guid.Parse(numberDto.Id);
                    var phoneNumber = existingVacancy.Vacancy.VacancyNumbers?.FirstOrDefault(p =>
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
                .Applications.Where(a => a.VacancyId == vacancyGuid)
                .Select(a => a.UserId)
                .ToListAsync();

            await _publishEndpoint.Publish(
                new VacancyUpdatedEvent
                {
                    InformationId = vacancyGuid,
                    SenderId = _userGuid,
                    UserIds = userIds,
                    Content = $"Vakansiya yeniləndi: {existingVacancy.Vacancy.Title}",
                }
            );
        }

        /// <summary> Şirkət profilində vakansiya axtarışı vakansiya filterlere görə </summary>

        public async Task<ICollection<VacancyGetAllDto>> GetAllVacanciesAsync(
            string? titleName,
            string? categoryId,
            string? countryId,
            string? cityId,
            decimal? minSalary,
            decimal? maxSalary,
            int skip = 1,
            int take = 9
        )
        {
            var query = _context.Vacancies.AsNoTracking();
            query = ApplyVacancyFilters(
                query,
                titleName,
                categoryId,
                countryId,
                cityId,
                true,
                minSalary,
                maxSalary
            );

            var vacancies = await query
                .Select(v => new VacancyGetAllDto
                {
                    Id = v.Id,
                    Title = v.Title,
                    CompanyLogo = $"{_authServiceBaseUrl}/{v.CompanyLogo}",
                    CompanyName = v.CompanyName,
                    StartDate = v.StartDate,
                    Location = v.Location,
                    ViewCount = v.ViewCount,
                    IsActive = v.IsActive,
                    WorkType = v.WorkType,
                    WorkStyle = v.WorkStyle,
                    MainSalary = v.MainSalary,
                    MaxSalary = v.MaxSalary,
                })
                .Skip(Math.Max(0, (skip - 1) * take))
                .Take(take)
                .ToListAsync();

            return vacancies;
        }

        private static IQueryable<Vacancy> ApplyVacancyFilters(
            IQueryable<Vacancy> query,
            string? titleName,
            string? categoryId,
            string? countryId,
            string? cityId,
            bool? isActive,
            decimal? minSalary,
            decimal? maxSalary
        )
        {
            if (titleName != null)
                query = query.Where(x => x.Title.ToLower().Contains(titleName.ToLower()));

            if (isActive == true)
                query = query.Where(x => x.IsActive == true);

            if (isActive == false)
                query = query.Where(x => x.IsActive == false);

            if (minSalary != null && maxSalary != null)
                query = query.Where(x => x.MainSalary >= minSalary && x.MaxSalary <= maxSalary);

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
    }
}
