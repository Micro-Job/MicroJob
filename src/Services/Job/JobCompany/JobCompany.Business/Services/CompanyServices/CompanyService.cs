using System.Security.Claims;
using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.CompanyDtos;
using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Exceptions.Common;
using JobCompany.Business.Exceptions.UserExceptions;
using JobCompany.Business.Extensions;
using JobCompany.Business.Statistics;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Exceptions;
using Shared.Requests;
using Shared.Responses;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.Enums;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Statics;

namespace JobCompany.Business.Services.CompanyServices
{
    public class CompanyService : ICompanyService
    {
        private JobCompanyDbContext _context;
        readonly IConfiguration _configuration;
        private readonly string? _authServiceBaseUrl;
        private readonly ICurrentUser _currentUser;
        private readonly IFileService _fileService;
        private readonly IHttpContextAccessor _contextAccessor;

        public CompanyService(
            JobCompanyDbContext context,
            IHttpContextAccessor contextAccessor,
            IConfiguration configuration, ICurrentUser currentUser
            , IFileService fileService
        )
        {
            _context = context;
            _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
            _configuration = configuration;
            _currentUser = currentUser;
            _fileService = fileService;
            _contextAccessor = contextAccessor;
        }

        public async Task UpdateCompanyAsync(CompanyUpdateDto dto, ICollection<UpdateNumberDto>? numbersDto)
        {
            var company = await _context.Companies.Include(c => c.CompanyNumbers).FirstOrDefaultAsync(x => x.UserId == _currentUser.UserGuid)
                ?? throw new SharedLibrary.Exceptions.NotFoundException<Company>(MessageHelper.GetMessage("NOT_FOUND"));

            company.CompanyName = dto.CompanyName?.Trim();
            company.CompanyInformation = dto.CompanyInformation?.Trim();
            company.CompanyLocation = dto.CompanyLocation?.Trim();
            company.WebLink = dto.WebLink;
            company.EmployeeCount = dto.EmployeeCount ?? company.EmployeeCount;
            company.CreatedDate = dto.CreatedDate;
            company.CategoryId = dto.CategoryId;
            company.CountryId = dto.CountryId;
            company.CityId = dto.CityId;

            if (dto.Email != null && await _context.Companies.AnyAsync(x => x.Email == dto.Email && x.Id != company.Id))
                throw new EmailAlreadyUsedException(MessageHelper.GetMessage("EMAIL_ALREADY_USED"));

            company.Email = dto.Email;

            if (!string.IsNullOrEmpty(company.CompanyLogo))
            {
                _fileService.DeleteFile(company.CompanyLogo);
            }

            if (dto.CompanyLogo != null)
            {
                FileDto fileResult = await _fileService.UploadAsync(FilePaths.image, dto.CompanyLogo);

                company.CompanyLogo = $"{fileResult.FilePath}/{fileResult.FileName}";
            }

            if (numbersDto is not null)
            {
                var numbersDtoDict = numbersDto
                    .Where(n => n.Id != null)
                    .ToDictionary(n => n.Id!.ToString(), n => n.PhoneNumber);

                // Id-si olmayan nömrələr yeni nömrələrdir. Onları əlavə edir
                var numbersToAdd = numbersDto
                    .Where(n => n.Id == null)
                    .Select(n => new CompanyNumber { Number = n.PhoneNumber, CompanyId = company.Id })
                    .ToList();

                // Mövcud nömrələri alırıq
                var existingNumbers = company.CompanyNumbers?.ToDictionary(
                    n => n.Id.ToString(),
                    n => n
                ) ?? [];

                var numbersToRemove = new List<CompanyNumber>();

                foreach (var kvp in existingNumbers)
                {
                    // Yeni nömrələr arasında mövcud nömrə yoxdursa mövcud nömrəni silir
                    if (!numbersDtoDict.TryGetValue(kvp.Key, out string? value))
                    {
                        numbersToRemove.Add(kvp.Value);
                    }
                    else
                    {
                        // Yeni nömrə mövcud nömrədən fərqli olduqda mövcud olan nömrəni yeniləyir
                        kvp.Value.Number = value;
                        numbersDtoDict.Remove(kvp.Key);
                    }
                }
                await _context.CompanyNumbers.AddRangeAsync(numbersToAdd);
                _context.CompanyNumbers.RemoveRange(numbersToRemove);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<DataListDto<CompanyDto>> GetAllCompaniesAsync(string? searchTerm, int skip = 1, int take = 12)
        {
            var query = _context.Companies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(x => x.CompanyName.Contains(searchTerm));

            var companies = await query
                .Select(c => new CompanyDto
                {
                    CompanyId = c.Id,
                    CompanyName = c.CompanyName,
                    CompanyImage = c.CompanyLogo != null ? $"{_authServiceBaseUrl}/{c.CompanyLogo}" : null,
                    CompanyVacancyCount = c.Vacancies != null ? c.Vacancies.Count(v => v.VacancyStatus == VacancyStatus.Active && v.EndDate > DateTime.Now) : 0,
                })
                .Skip(Math.Max(0, (skip - 1) * take))
                .Take(take)
                .ToListAsync();

            var count = await query.CountAsync();

            return new DataListDto<CompanyDto>
            {
                Datas = companies,
                TotalCount = count,
                //TotalPage = (int)Math.Ceiling((double)count / take)
            };
        }

        //TODO : bu hissede email ve phonenumber ucun job proyektindeki endpointe sorgu atmaq mumkundur mu?
        /// <summary> İdyə görə şirkət detaili consumer metodundan istifadə ilə </summary>
        public async Task<CompanyDetailItemDto> GetCompanyDetailAsync(string id)
        {
            var companyGuid = Guid.Parse(id);
            var company = await _context.Companies.Where(c => c.Id == companyGuid)
                        .Select(x => new CompanyDetailItemDto
                        {
                            CompanyInformation = x.CompanyInformation,
                            CompanyLocation = x.CompanyLocation,
                            CompanyName = x.CompanyName,
                            CompanyLogo = $"{_authServiceBaseUrl}/{x.CompanyLogo}",
                            WebLink = x.WebLink,
                            UserId = x.UserId,
                            CompanyNumbers = x.CompanyNumbers
                                            .Select(cn => new CompanyNumberDto
                                            {
                                                Id = cn.Id,
                                                Number = cn.Number,
                                            })
                                            .ToList(),
                            Email = x.Email,
                        })
                        .FirstOrDefaultAsync()
                    ?? throw new SharedLibrary.Exceptions.NotFoundException<Company>(MessageHelper.GetMessage("NOT_FOUND"));
            return company;
        }

        /// <summary>
        /// Şirkət profili 
        /// </summary>
        public async Task<CompanyProfileDto> GetOwnCompanyInformationAsync()
        {
            var currentLanguage = _currentUser.LanguageCode;

            //Bu application-ın run olduğu serverin url-ini alırıq (Məs: http://localhost:5082)
            var serviceUrl = $"{_contextAccessor.HttpContext?.Request.Scheme}://{_contextAccessor.HttpContext?.Request.Host}";
            Console.WriteLine(serviceUrl);
            var companyProfile = await _context.Companies
                .Where(c => c.UserId == _currentUser.UserGuid)
                .Include(x => x.Category.Translations)
                .Include(x => x.City.Translations)
                .Include(x => x.Country.Translations)
                .Select(x => new CompanyProfileDto
                {
                    Id = x.Id,
                    Name = x.CompanyName,
                    Information = x.CompanyInformation,
                    Location = x.CompanyLocation,
                    WebLink = x.WebLink,
                    CreatedDate = x.CreatedDate,
                    EmployeeCount = x.EmployeeCount.HasValue ? x.EmployeeCount.Value : null,
                    CompanyLogo = !string.IsNullOrEmpty(x.CompanyLogo) ? $"{serviceUrl}/{x.CompanyLogo}" : null,
                    Category = x.Category.GetTranslation(currentLanguage, GetTranslationPropertyName.Name),
                    City = x.City != null ? x.City.GetTranslation(currentLanguage, GetTranslationPropertyName.Name) : null,
                    Country = x.Country != null ? x.Country.GetTranslation(currentLanguage, GetTranslationPropertyName.Name) : null,
                    CategoryId = x.CategoryId,
                    CityId = x.CityId,
                    CountryId = x.CountryId,
                    Email = x.Email,
                    CompanyNumbers = x.CompanyNumbers != null
                        ? x.CompanyNumbers.Select(cn => new CompanyNumberDto
                        {
                            Id = cn.Id,
                            Number = cn.Number,
                        }).ToList()
                        : new List<CompanyNumberDto>()
                })
                .FirstOrDefaultAsync()
                    ?? throw new SharedLibrary.Exceptions.NotFoundException<Company>(MessageHelper.GetMessage("NOT_FOUND"));

            return companyProfile;
        }

        public async Task<string?> GetCompanyNameAsync(string companyId)
        {
            var companyGuid = Guid.Parse(companyId);

            return await _context.Companies.Where(x => x.Id == companyGuid)
                .Select(x => x.CompanyName)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException<Company>("Şirkət mövcud deyil");
        }
    }
}
