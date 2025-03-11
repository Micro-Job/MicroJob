using JobCompany.Business.Dtos.CountryDtos;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace JobCompany.Business.Services.CountryServices
{
    public class CountryService(JobCompanyDbContext context) : ICountryService
    {
        readonly JobCompanyDbContext _context = context;

        public async Task CreateCountryAsync(string countryName)
        {
            var existCountry = await _context.Countries
                                              .FirstOrDefaultAsync(c => c.Name == countryName);
            if (existCountry != null) throw new Exceptions.Common.IsAlreadyExistException<Country>();

            var country = new Country
            {
                Name = countryName,
            };

            await _context.Countries.AddAsync(country);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCountryAsync(string id)
        {
            var countryId = Guid.Parse(id);
            var country = await _context.Countries.FindAsync(countryId)
                ?? throw new Exceptions.Common.NotFoundException<Country>();

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<CountryListDto>> GetAllCountryAsync()
        {
            var countries = await _context.Countries.Select(c => new CountryListDto
            {
                Id = c.Id,
                CountryName = c.Name,
            }).ToListAsync();
            return countries;
        }

        public async Task UpdateCountryAsync(string id, string? countryName)
        {
            var countryId = Guid.Parse(id);
            var country = await _context.Countries.FindAsync(countryId)
            ?? throw new Exceptions.Common.NotFoundException<Country>();

            var isExistCountry = await _context.Countries.FirstOrDefaultAsync(c => c.Name == countryName && c.Id != countryId);
            if (isExistCountry != null) throw new Exceptions.Common.IsAlreadyExistException<Country>();

            country.Name = countryName;
            await _context.SaveChangesAsync();
        }
    }
}