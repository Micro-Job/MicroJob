using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Business.Dtos;
using AuthService.Business.Services.CurrentUser;
using AuthService.DAL.Contexts;
using JobCompany.Business.Dtos.CompanyDtos;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using Shared.Exceptions;
using SharedLibrary.Exceptions;

namespace JobCompany.Business.Services.Company
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
            userGuid = Guid.Parse(_currentUser.UserId);
        }

        public async Task UpdateCompanyAsync(Dtos.CompanyDtos.CompanyUpdateDto dto)
        {
            var company = await _context.Companies.FindAsync(userGuid) 
            ?? throw new NotFoundException<Core.Entites.Company>();

            var companyName = dto.CompanyName.Trim();

            var existingCompany = await _context.Companies.FirstOrDefaultAsync(x=>x.CompanyName == companyName && x.UserId != userGuid);
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

    }
}