using System.Security.Claims;
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

        public CompanyService(
            JobCompanyDbContext context,
            IRequestClient<GetAllCompaniesDataRequest> client,
            IHttpContextAccessor contextAccessor,
            IConfiguration configuration
        )
        {
            _context = context;
            _client = client;
            _contextAccessor = contextAccessor;
            _baseUrl =
                $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host.Value}{_contextAccessor.HttpContext.Request.PathBase.Value}";
            _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
            _configuration = configuration;
        }

        public async Task UpdateCompanyAsync(
            CompanyUpdateDto dto,
            ICollection<UpdateNumberDto>? numbersDto
        )
        {
            Guid userGuid = Guid.Parse(
                _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid).Value
            );

            var company =
                await _context
                    .Companies.Include(c => c.CompanyNumbers)
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
                var numbersDtoDict = numbersDto.ToDictionary(n => n.Id, n => n.PhoneNumber);

                var existingNumbers = company.CompanyNumbers.ToDictionary(
                    n => n.Id.ToString(),
                    n => n
                );

                foreach (var kvp in existingNumbers)
                {
                    if (!numbersDtoDict.ContainsKey(kvp.Key))
                    {
                        company.CompanyNumbers.Remove(kvp.Value);
                    }
                    else
                    {
                        kvp.Value.Number = numbersDtoDict[kvp.Key];
                        numbersDtoDict.Remove(kvp.Key);
                    }
                }

                foreach (var newNumber in numbersDtoDict)
                {
                    company.CompanyNumbers.Add(
                        new CompanyNumber { Number = newNumber.Value, CompanyId = company.Id }
                    );
                }
            }

            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<CompanyListDto>> GetAllCompaniesAsync(
            string? searchTerm,
            int skip = 1,
            int take = 12
        )
        {
            var query = _context.Companies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    c.CompanyName.ToLower().Contains(searchTerm.Trim().ToLower())
                );
            }

            var companies = await query
                .Select(c => new CompanyListDto
                {
                    CompanyId = c.Id,
                    CompanyName = c.CompanyName,
                    CompanyImage = $"{_authServiceBaseUrl}/{c.CompanyLogo}",
                    CompanyVacancyCount = c.Vacancies.Count(v => v.IsActive),
                })
                .Skip(Math.Max(0, (skip - 1) * take))
                .Take(take)
                .ToListAsync();

            return companies;
        }

        /// <summary> Consumer metodu - User id ye görə bütün şirkətlərin datalarının gətirilməsi </summary>
        public async Task<GetAllCompaniesDataResponse> GetAllCompaniesDataResponseAsync(Guid UserId)
        {
            var response = await _client.GetResponse<GetAllCompaniesDataResponse>(
                new GetAllCompaniesDataRequest { UserId = UserId }
            );
            return response.Message;
        }

        /// <summary> İdyə görə şirkət detaili consumer metodundan istifadə ilə </summary>
        public async Task<CompanyDetailItemDto> GetCompanyDetailAsync(string id)
        {
            var companyGuid = Guid.Parse(id);
            var company =
                await _context
                    .Companies.Where(c => c.Id == companyGuid)
                    .Select(x => new CompanyDetailItemDto
                    {
                        CompanyInformation = x.CompanyInformation,
                        CompanyLocation = x.CompanyLocation,
                        CompanyName = x.CompanyName,
                        CompanyLogo = $"{_authServiceBaseUrl}/{x.CompanyLogo}",
                        WebLink = x.WebLink,
                        UserId = x.UserId.ToString(),
                        CompanyNumbers = x
                            .CompanyNumbers.Select(cn => new CompanyNumberDto
                            {
                                Id = cn.Id,
                                Number = cn.Number,
                            })
                            .ToList(),
                    })
                    .FirstOrDefaultAsync()
                ?? throw new SharedLibrary.Exceptions.NotFoundException<Company>();

            var GuidUserId = Guid.Parse(company.UserId);

            var response = await GetAllCompaniesDataResponseAsync(GuidUserId);
            company.Email = response.Email;
            company.PhoneNumber = response.PhoneNumber;
            return company;
        }
    }
}
