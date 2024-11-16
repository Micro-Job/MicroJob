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
            var company = await _context.Companies.FindAsync(vacancyDto.CompanyId);

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
                CompanyId = vacancyDto.CompanyId,
                CompanyName = vacancyDto.CompanyName,
                Title = vacancyDto.Title,
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
            await _context.Vacancies.AddAsync(vacancy);

            var numbers = new List<CompanyNumber>();
            if (numberDto is not null)
            {
                foreach (var numberCreateDto in numberDto)
                {
                    var number = new Core.Entites.CompanyNumber
                    {
                        Number = numberCreateDto.PhoneNumber,
                    };
                    numbers.Add(number);
                }
            }
            await _context.CompanyNumbers.AddRangeAsync(numbers);

            vacancy.CompanyNumbers = numbers;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var vacancyGuid = Guid.Parse(id);
            var vacancy = await _context.Vacancies
                                        .Where(x => x.Id == vacancyGuid && x.Company.UserId == _userGuid)
                                        .Select(x => new { x.Id, x.IsActive })
                                        .FirstOrDefaultAsync() ?? throw new NotFoundException<Vacancy>();

            var vacancyToUpdate = new Vacancy { Id = vacancyGuid, IsActive = false };
            _context.Vacancies.Attach(vacancyToUpdate);
            _context.Entry(vacancyToUpdate).Property(x => x.IsActive).IsModified = true;
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

        public async Task<VacancyGetByIdDto> GetByIdVacancyAsync(string id)
        {
            var vacancyGuid = Guid.Parse(id);
            var vacancy = await _context.Vacancies.FirstOrDefaultAsync(x => x.Id == vacancyGuid && x.Company.UserId == _userGuid).Select(x => new VacancyGetByIdDto
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