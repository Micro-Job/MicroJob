using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.CityDtos;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace JobCompany.Business.Services.CityServices
{
    public class CityService : ICityService
    {
        readonly JobCompanyDbContext _context;

        public CityService(JobCompanyDbContext context)
        {
            _context = context;
        }

        public async Task CreateCityAsync(CreateCityDto dto)
        {
            var GuidCountrId = Guid.Parse(dto.CountryId);
            var existCity = await _context.Cities.FindAsync(dto.CityName);
            if (existCity != null) throw new Exceptions.Common.IsAlreadyExistException<City>();

            var isExistCountry = await _context.Countries.FindAsync(GuidCountrId);
            if (isExistCountry == null) throw new Exceptions.Common.NotFoundException<Country>();

            var city = new City
            {
                CityName = dto.CityName,
                CountryId = GuidCountrId,
            };

            _context.Cities.Add(city);
        }

        public async Task<ICollection<CityListDto>> GetAllCitiesAsync()
        {
            var cities = await _context.Cities
               .Select(c => new CityListDto
               {
                   CityName = c.CityName,
                   CountryId = c.CountryId
               })
               .ToListAsync();

            return cities;
        }

        public async Task UpdateCityAsync(string id, UpdateCityDto dto)
        {
            var cityId = Guid.Parse(id);
            var city = await _context.Cities.FindAsync(cityId)
            ?? throw new Exceptions.Common.NotFoundException<City>();

            city.CityName = dto.CityName;
        }

        public async Task<ICollection<CityNameDto>> GetAllCitiesByCountryIdAsync(string countryId)
        {
            var guidCountryId = Guid.Parse(countryId);
            var existCountry = await _context.Cities.FindAsync(guidCountryId) 
            ?? throw new Exceptions.Common.NotFoundException<Country>();

            var cities = await _context.Cities
            .Where(city => city.CountryId == guidCountryId)
            .Select(city => new CityNameDto
            {
                CityName = city.CityName,
            })
            .ToListAsync();

            return cities;
        }
    }
}