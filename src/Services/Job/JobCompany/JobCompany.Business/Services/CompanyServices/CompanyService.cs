using AuthService.Business.Exceptions.UserException;
using AuthService.Business.Services.CurrentUser;
using JobCompany.Business.Dtos.CompanyDtos;
using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using Shared.Exceptions;
using Shared.Responses;
using SharedLibrary.Exceptions;
using SharedLibrary.Responses;

namespace JobCompany.Business.Services.CompanyServices
{
    public class CompanyService : ICompanyService
    {
        private JobCompanyDbContext _context;
        readonly ICurrentUser _currentUser;
        private readonly Guid userGuid;


        public CompanyService(JobCompanyDbContext context, ICurrentUser currentUser)
        {
            _context = context;
            _currentUser = currentUser;
            userGuid = Guid.Parse(_currentUser.UserId ?? throw new UserNotLoggedInException());
        }

        public async Task UpdateCompanyAsync(CompanyUpdateDto dto)
        {
            var company = await _context.Companies.FindAsync(userGuid)
            ?? throw new NotFoundException<Company>();

            var companyName = dto.CompanyName.Trim();

            var existingCompany = await _context.Companies.FirstOrDefaultAsync(x => x.CompanyName == companyName && x.UserId != userGuid);
            if (existingCompany != null) throw new MustBeUniqueException<Core.Entites.Company>();

            company.CompanyName = dto.CompanyName;
            company.CompanyInformation = dto.CompanyInformation;
            company.CompanyLocation = dto.CompanyLocation;
            company.WebLink = dto.WebLink;
            company.EmployeeCount = dto.EmployeeCount.Value;
            company.CreatedDate = dto.CreatedDate;

            var category = await _context.Categories.FindAsync(dto.CategoryId)
            ?? throw new NotFoundException<Category>();
            company.CategoryId = category.Id;

            var country = await _context.Countries.FindAsync(dto.CountryId)
            ?? throw new NotFoundException<Country>();
            company.CountryId = country.Id;

            var city = await _context.Cities.FindAsync(dto.CityId)
            ?? throw new NotFoundException<City>();
            company.CityId = city.Id;

            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<CompanyListDto>> GetAllCompanies()
        {
            var companies = await _context.Companies.Include(x=>x.Vacancies).Select(c => new CompanyListDto
                {
                    CompanyId = c.Id,
                    CompanyName = c.CompanyName,
                    CompanyImage = c.CompanyLogo,
                    CompanyVacancyCount = c.Vacancies.Count(v => v.IsActive)
                })
                .ToListAsync();

            return companies;
        }

        public async Task<CompanyDetailItemDto> GetCompanyDetailAsync(string id)
        {
            var companyGuid = Guid.Parse(id);
            var company = await  _context.Companies
            .Where(c => c.Id == companyGuid)
            .Select(x => new CompanyDetailItemDto
            {
                CompanyInformation = x.CompanyInformation,
                CompanyLocation = x.CompanyLocation,
                WebLink = x.WebLink,
                CompanyNumbers = x.CompanyNumbers.Select(cn => new CompanyNumberDto
                {
                    Number = cn.Number,
                }).ToList()
            }).FirstOrDefaultAsync() 
            ?? throw new NotFoundException<Company>();

            // yazilacaq
            // company.Email
            // company.PhoneNumber
            return company;
        }
    }
}