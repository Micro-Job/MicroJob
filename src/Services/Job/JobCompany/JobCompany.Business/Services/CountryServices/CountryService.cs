using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.CountryDtos;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace JobCompany.Business.Services.CountryServices
{
    public class CountryService : ICountryService
    {
        readonly JobCompanyDbContext _context;

        public CountryService(JobCompanyDbContext context)
        {
            _context = context;
        }

        public async Task CreateCountryAsync(CreateCountryDto dto)
        {
            var existCountry = await _context.Categories.FindAsync(dto.CountryName);
            if (existCountry != null) throw new Exceptions.Common.IsAlreadyExistException<Country>();

            var country = new Country
            {
                CountryName = dto.CountryName,
            };

            _context.Countries.Add(country);
        }

        public async Task DeleteCountryAsync(string id)
        {
            var countryId = Guid.Parse(id);
            var country = await _context.Countries.FindAsync(countryId)
                ?? throw new Exceptions.Common.NotFoundException<Country>();

            _context.Countries.Remove(country);
        }

        public async Task<ICollection<CountryListDto>> GetAllCountryAsync()
        {
            var countries = await _context.Countries.Select(c => new CountryListDto
            {
                CountryName = c.CountryName,
            }).ToListAsync();
            return countries;
        }

        public async Task UpdateCountryAsync(string id, UpdateCountryDto dto)
        {
            var countryId = Guid.Parse(id);
            var country = await _context.Countries.FindAsync(countryId)
            ?? throw new Exceptions.Common.NotFoundException<Country>();

            var isExistCountry = await _context.Countries.FirstOrDefaultAsync(c => c.CountryName == dto.CountryName && c.Id != countryId);
            if (isExistCountry != null) throw new Exceptions.Common.IsAlreadyExistException<Country>();

            country.CountryName = dto.CountryName;
        }
    }
}