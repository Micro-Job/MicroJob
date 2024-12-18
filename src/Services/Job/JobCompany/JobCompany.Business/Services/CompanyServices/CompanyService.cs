using JobCompany.Business.Dtos.CompanyDtos;
using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Exceptions.Common;
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
using SharedLibrary.Exceptions;
using SharedLibrary.Responses;
using System.Security.Claims;

namespace JobCompany.Business.Services.CompanyServices
{
    public class CompanyService : ICompanyService
    {
        private JobCompanyDbContext _context;
        readonly IConfiguration _configuration;
        readonly IRequestClient<GetAllCompaniesDataRequest> _client;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string? _baseUrl;
        private readonly string? _authServiceBaseUrl;


        public CompanyService(JobCompanyDbContext context, IRequestClient<GetAllCompaniesDataRequest> client, IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            _context = context;
            _client = client;
            _contextAccessor = contextAccessor;
            _baseUrl = $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host.Value}{_contextAccessor.HttpContext.Request.PathBase.Value}";
            _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
            _configuration = configuration;
        }

        public async Task UpdateCompanyAsync(CompanyUpdateDto dto, ICollection<CreateNumberDto>? numbersDto)
        {
            Guid userGuid = Guid.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid).Value);
            var company = await _context.Companies
                .Include(c => c.CompanyNumbers)
                .FirstOrDefaultAsync(x => x.UserId == userGuid)
                ?? throw new SharedLibrary.Exceptions.NotFoundException<Company>();

            company.CompanyName = dto.CompanyName.Trim();
            company.CompanyInformation = dto.CompanyInformation.Trim();
            company.CompanyLocation = dto.CompanyLocation.Trim();
            company.WebLink = dto.WebLink;
            company.EmployeeCount = dto.EmployeeCount ?? company.EmployeeCount;
            company.CreatedDate = dto.CreatedDate;
            company.CategoryId = dto.CategoryId;
            company.CountryId = dto.CountryId;
            company.CityId = dto.CityId;

            if (numbersDto is not null)
            {
                foreach (var numberDto in numbersDto)
                {
                    if (company.CompanyNumbers.Any(n => n.Number == numberDto.PhoneNumber))
                    {
                        throw new IsAlreadyExistException<CompanyNumber>($"Number {numberDto.PhoneNumber} already exists.");
                    }

                    var newNumber = new CompanyNumber
                    {
                        Number = numberDto.PhoneNumber,
                        CompanyId = company.Id
                    };

                    company.CompanyNumbers.Add(newNumber);
                }
            }

            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<CompanyListDto>> GetAllCompaniesAsync(string? searchTerm, int skip = 1, int take = 12)
        {
            var query = _context.Companies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => c.CompanyName.ToLower().Contains(searchTerm.Trim().ToLower()));
            }

            var companies = await query
                .Select(c => new CompanyListDto
                {
                    CompanyId = c.Id,
                    CompanyName = c.CompanyName,
                    CompanyImage = $"{_authServiceBaseUrl}/{c.CompanyLogo}",
                    CompanyVacancyCount = c.Vacancies.Count(v => v.IsActive)
                })
            .Skip(Math.Max(0, (skip - 1) * take))
            .Take(take)
            .ToListAsync();

            return companies;
        }

        /// <summary> Consumer metodu - User id ye görə bütün şirkətlərin datalarının gətirilməsi </summary>
        public async Task<GetAllCompaniesDataResponse> GetAllCompaniesDataResponseAsync(Guid UserId)
        {
            var response = await _client.GetResponse<GetAllCompaniesDataResponse>(new GetAllCompaniesDataRequest { UserId = UserId });
            return response.Message;
        }

        /// <summary> İdyə görə şirkət detaili consumer metodundan istifadə ilə </summary>
        public async Task<CompanyDetailItemDto> GetCompanyDetailAsync(string id)
        {
            var companyGuid = Guid.Parse(id);
            var company = await _context.Companies
            .Where(c => c.Id == companyGuid)
            .Select(x => new CompanyDetailItemDto
            {
                CompanyInformation = x.CompanyInformation,
                CompanyLocation = x.CompanyLocation,
                WebLink = x.WebLink,
                UserId = x.UserId.ToString(),
                CompanyNumbers = x.CompanyNumbers.Select(cn => new CompanyNumberDto
                {
                    Number = cn.Number,
                }).ToList()
            }).FirstOrDefaultAsync()
            ?? throw new SharedLibrary.Exceptions.NotFoundException<Company>();

            var GuidUserId = Guid.Parse(company.UserId);

            var response = await GetAllCompaniesDataResponseAsync(GuidUserId);
            company.Email = response.Email;

            return company;
        }
    }
}