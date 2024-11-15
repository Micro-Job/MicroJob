using AuthService.Business.Services.CurrentUser;
using AuthService.Core.Entities;
using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.DAL.Contexts;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.Exceptions;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Statics;

namespace JobCompany.Business.Services.Vacancy
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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                FileDto fileResult = vacancyDto.CompanyLogo != null
                    ? await _fileService.UploadAsync(FilePaths.image, vacancyDto.CompanyLogo) : new();

                var vacancy = new Core.Entites.Vacancy
                {
                    Id = Guid.NewGuid(),
                    CompanyId = vacancyDto.CompanyId,
                    CompanyName = vacancyDto.CompanyName,
                    Title = vacancyDto.Title,
                    CompanyLogo = vacancyDto.CompanyLogo != null
                        ? $"{fileResult.FilePath}/{fileResult.FileName}" : null,
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

                var numbers = new List<Core.Entites.CompanyNumber>();
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
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}