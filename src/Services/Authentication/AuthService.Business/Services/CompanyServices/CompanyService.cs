using AuthService.Business.Dtos;
using AuthService.Business.Services.CurrentUser;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Exceptions;

namespace AuthService.Business.Services.CompanyServices
{
    public class CompanyService(JobCompanyDbContext context, ICurrentUser currentUser) : ICompanyService
    {
        private readonly JobCompanyDbContext _context = context;
        private readonly ICurrentUser _currentUser = currentUser;
        private readonly Guid userGuid;


        public async Task UpdateCompanyAsync(CompanyUpdateDto dto)
        {
            var companyGuid = Guid.Parse(dto.Id);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == companyGuid)
               ?? throw new NotFoundException<Company>();
            company.CompanyInformation = dto.CompanyInformation;
        }
    }
}