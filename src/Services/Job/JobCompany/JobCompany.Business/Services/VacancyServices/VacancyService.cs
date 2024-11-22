using AuthService.Business.Services.CurrentUser;
using AuthService.Core.Entities;
using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.Exceptions;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Statics;

namespace JobCompany.Business.Services.VacancyServices
{
    public class VacancyService : IVacancyService
    {
        private readonly JobCompanyDbContext _context;
        private readonly ICurrentUser _currentUser;
        private readonly Guid _userGuid;
        private readonly IFileService _fileService;

        public VacancyService(JobCompanyDbContext context, ICurrentUser currentUser, IFileService fileService)
        {
            _context = context;
            _currentUser = currentUser;
            _fileService = fileService;
            _userGuid = Guid.Parse(_currentUser.UserId ?? throw new NotFoundException<User>());
        }

        public async Task CreateVacancyAsync(CreateVacancyDto vacancyDto, ICollection<CreateNumberDto>? numberDto)
        {
            string? companyLogoPath = null;
            var company = await _context.Companies.FirstOrDefaultAsync(x => x.UserId == _userGuid);

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
                CompanyName = vacancyDto.CompanyName.Trim(),
                Title = vacancyDto.Title.Trim(),
                CompanyLogo = companyLogoPath,
                StartDate = vacancyDto.StartDate,
                EndDate = vacancyDto.EndDate,
                Location = vacancyDto.Location,
                CountryId = vacancyDto.CountryId,
                CityId = vacancyDto.CityId,
                Email = vacancyDto.Email,
                WorkType = vacancyDto.WorkType,
                MainSalary = vacancyDto.MainSalary,
                MaxSalary = vacancyDto.MaxSalary,
                Requirement = vacancyDto.Requirement,
                Description = vacancyDto.Description,
                Gender = vacancyDto.Gender,
                Military = vacancyDto.Military,
                Driver = vacancyDto.Driver,
                Family = vacancyDto.Family,
                Citizenship = vacancyDto.Citizenship,
                CategoryId = vacancyDto.CategoryId
            };

            var numbers = new List<CompanyNumber>();
            if (numberDto is not null)
            {
                foreach (var numberCreateDto in numberDto)
                {
                    var number = new CompanyNumber
                    {
                        Number = numberCreateDto.PhoneNumber,
                    };
                    numbers.Add(number);
                }
            }
            await _context.CompanyNumbers.AddRangeAsync(numbers);

            vacancy.CompanyNumbers = numbers;
            await _context.Vacancies.AddAsync(vacancy);
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
            }).ToList();

            var deletedCount = await _context.Vacancies
        .Where(x => vacancyGuids.Contains(x.Id) && x.Company.UserId == _userGuid)
        .ExecuteUpdateAsync(x => x.SetProperty(v => v.IsActive, false));

            if (deletedCount == 0)
                throw new NotFoundException<Vacancy>();
        }

        public async Task<List<VacancyGetAllDto>> GetAllVacanciesAsync()
        {
            var vacancies = await _context.Vacancies.Where(x => x.Company.UserId == _userGuid && x.IsActive).Select(x => new VacancyGetAllDto
            {
                Id = x.Id,
                Title = x.Title,
                CompanyLogo = x.CompanyLogo,
                StartDate = x.StartDate,
                Location = x.Location,
                ViewCount = x.ViewCount,
                WorkType = x.WorkType,
                MainSalary = x.MainSalary,
                MaxSalary = x.MaxSalary,
            }).ToListAsync();
            return vacancies;
        }

        public async Task<List<VacancyListDtoForAppDto>> GetAllVacanciesForAppAsync()
        {
            var vacancies = await _context.Vacancies.Where(x=>x.Company.UserId == _userGuid && x.IsActive == true).Select(x=> new VacancyListDtoForAppDto
            {
                VacancyId = x.Id,
                VacancyName = x.Title
            }).ToListAsync();

            return vacancies;
        }

        public async Task<ICollection<VacancyGetByCompanyIdDto>> GetVacancyByCompanyIdAsync(string companyId)
        {
            var companyGuid = Guid.Parse(companyId);
            var vacancies = await _context.Vacancies.Where(x => x.Company.UserId == _userGuid && x.IsActive == true).Select(x => new VacancyGetByCompanyIdDto
            {
                CompanyName = x.CompanyName,
                Title = x.Title,
                Location = x.Location,
                WorkType = x.WorkType,
                StartDate = x.StartDate,
                ViewCount = x.ViewCount,
                MainSalary = x.MainSalary,
                MaxSalary = x.MaxSalary
            }).ToListAsync();

            return vacancies; 
        }

        public async Task<VacancyGetByIdDto> GetByIdVacancyAsync(string id)
        {
            var vacancyGuid = Guid.Parse(id);
            var vacancy = await _context.Vacancies.FirstOrDefaultAsync(x => x.Id == vacancyGuid).Select(x => new VacancyGetByIdDto
            {
                Id = x.Id,
                Title = x.Title,
                CompanyLogo = x.CompanyLogo,
                StartDate = x.StartDate,
                Location = x.Location,
                ViewCount = x.ViewCount,
                WorkType = x.WorkType,
                MainSalary = x.MainSalary,
                MaxSalary = x.MaxSalary,
                Requirement = x.Requirement,
                Description = x.Description,
                Gender = x.Gender,
                Military = x.Military,
                Family = x.Family,
                Driver = x.Driver,
                Citizenship = x.Citizenship,
                CompanyNumbers = x.CompanyNumbers,

                Country = new Country
                {
                    CountryName = x.Country.CountryName
                },

                Company = new Core.Entites.Company
                {
                    CompanyName = x.CompanyName,
                    CompanyInformation = x.Company.CompanyInformation
                },

                Category = new Category
                {
                    CategoryName = x.Category.CategoryName
                }
            }) ?? throw new NotFoundException<Vacancy>();
            return vacancy;
        }

        public async Task UpdateVacancyAsync(UpdateVacancyDto vacancyDto, ICollection<UpdateNumberDto>? numberDtos)
        {
            var existingVacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Company.UserId == _userGuid)
                ?? throw new NotFoundException<Vacancy>();

            existingVacancy.CompanyId = Guid.Parse(vacancyDto.CompanyId);
            existingVacancy.CompanyName = vacancyDto.CompanyName;
            existingVacancy.Title = vacancyDto.Title;
            existingVacancy.StartDate = vacancyDto.StartDate;
            existingVacancy.EndDate = vacancyDto.EndDate;
            existingVacancy.Location = vacancyDto.Location;
            existingVacancy.CountryId = Guid.Parse(vacancyDto.CountryId ?? throw new Exception());
            existingVacancy.CityId = Guid.Parse(vacancyDto.CityId ?? throw new Exception());
            existingVacancy.Email = vacancyDto.Email;
            existingVacancy.WorkType = vacancyDto.WorkType;
            existingVacancy.MainSalary = vacancyDto.MainSalary;
            existingVacancy.MaxSalary = vacancyDto.MaxSalary;
            existingVacancy.Requirement = vacancyDto.Requirement;
            existingVacancy.Description = vacancyDto.Description;
            existingVacancy.Gender = vacancyDto.Gender;
            existingVacancy.Military = vacancyDto.Military;
            existingVacancy.Driver = vacancyDto.Driver;
            existingVacancy.Family = vacancyDto.Family;
            existingVacancy.Citizenship = vacancyDto.Citizenship;
            existingVacancy.CategoryId = Guid.Parse(vacancyDto.CategoryId ?? throw new Exception());

            if (numberDtos is not null)
            {
                foreach (var numberDto in numberDtos)
                {
                    var phoneNumberGuid = Guid.Parse(numberDto.Id);
                    var phoneNumber = existingVacancy.CompanyNumbers?.FirstOrDefault(p => p.Id == phoneNumberGuid);
                    if (phoneNumber != null && phoneNumber.Number != numberDto.PhoneNumber)
                    {
                        phoneNumber.Number = numberDto.PhoneNumber;
                    }
                }
            }
            _context.Vacancies.Update(existingVacancy);
            await _context.SaveChangesAsync();
        }
    }
}