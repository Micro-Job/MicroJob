using System.Security.Claims;
using JobCompany.Business.Dtos.Common;
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
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.CompanyServices
{
    public class CompanyService : ICompanyService
    {
        private JobCompanyDbContext _context;
        readonly IConfiguration _configuration;
        readonly IRequestClient<GetAllCompaniesDataRequest> _client;
        private readonly string? _authServiceBaseUrl;
        private readonly ICurrentUser _currentUser;

        public CompanyService(
            JobCompanyDbContext context,
            IRequestClient<GetAllCompaniesDataRequest> client,
            IHttpContextAccessor contextAccessor,
            IConfiguration configuration,ICurrentUser currentUser
        )
        {
            _context = context;
            _client = client;
            _authServiceBaseUrl = configuration["AuthService:BaseUrl"];
            _configuration = configuration;
            _currentUser = currentUser;
        }

        public async Task UpdateCompanyAsync(CompanyUpdateDto dto,ICollection<UpdateNumberDto>? numbersDto)
        {
            var company = await _context.Companies.Include(c => c.CompanyNumbers).FirstOrDefaultAsync(x => x.UserId == _currentUser.UserGuid)
                ?? throw new SharedLibrary.Exceptions.NotFoundException<Company>(MessageHelper.GetMessage("NOT_FOUND"));

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

        public async Task<DataListDto<CompanyDto>> GetAllCompaniesAsync(string? searchTerm,int skip = 1,int take = 12)
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
                    CompanyVacancyCount = c.Vacancies != null ? c.Vacancies.Count(v => v.IsActive == SharedLibrary.Enums.VacancyStatus.Active) : 0,
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
                        })
                        .FirstOrDefaultAsync()
                    ?? throw new SharedLibrary.Exceptions.NotFoundException<Company>(MessageHelper.GetMessage("NOT_FOUND"));

            var response = await _client.GetResponse<GetAllCompaniesDataResponse>(
                new GetAllCompaniesDataRequest { UserId = company.UserId }
            );
            company.Email = response.Message.Email;
            company.PhoneNumber = response.Message.PhoneNumber;
            return company;
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
