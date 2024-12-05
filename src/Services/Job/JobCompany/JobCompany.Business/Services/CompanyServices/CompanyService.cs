using JobCompany.Business.Dtos.CompanyDtos;
using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        private readonly Guid userGuid;
        readonly IRequestClient<GetAllCompaniesDataRequest> _client;
        private readonly IHttpContextAccessor _contextAccessor;


        public CompanyService(JobCompanyDbContext context, IRequestClient<GetAllCompaniesDataRequest> client , IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _client = client;
            _contextAccessor = contextAccessor;
            userGuid = Guid.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid).Value);
        }

        public async Task UpdateCompanyAsync(CompanyUpdateDto dto)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(x=> x.UserId == userGuid) ?? throw new NotFoundException<Company>();

            var companyName = dto.CompanyName.Trim();

            company.CompanyName = dto.CompanyName;
            company.CompanyInformation = dto.CompanyInformation.Trim();
            company.CompanyLocation = dto.CompanyLocation.Trim();
            company.WebLink = dto.WebLink;
            company.EmployeeCount = dto.EmployeeCount.Value;
            company.CreatedDate = dto.CreatedDate;
            company.CategoryId = dto.CategoryId;
            company.CountryId = dto.CountryId;
            company.CityId = dto.CityId;

            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<CompanyListDto>> GetAllCompanies(int skip = 1, int take = 12)
        {
            var companies = await _context.Companies
                .Select(c => new CompanyListDto
                {
                    CompanyId = c.Id,
                    CompanyName = c.CompanyName,
                    CompanyImage = c.CompanyLogo,
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
            ?? throw new NotFoundException<Company>();

            var GuidUserId = Guid.Parse(company.UserId);

            var response = await GetAllCompaniesDataResponseAsync(GuidUserId);
            company.Email = response.Email;

            return company;
        }
    }
}