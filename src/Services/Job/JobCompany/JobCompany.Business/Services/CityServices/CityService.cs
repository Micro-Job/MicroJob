using JobCompany.Business.Dtos.CityDtos;
using JobCompany.Business.Exceptions.Common;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace JobCompany.Business.Services.CityServices
{
    public class CityService(JobCompanyDbContext context) : ICityService
    {
        readonly JobCompanyDbContext _context = context;

        public async Task CreateCityAsync(CreateCityDto dto)
        {
            var GuidCountrId = Guid.Parse(dto.CountryId);
            var existCity = await _context.Cities
                .SingleOrDefaultAsync(c => c.Name == dto.CityName);
            if (existCity != null) throw new IsAlreadyExistException<City>();

            var isExistCountry = await _context.Countries.FindAsync(GuidCountrId) ?? throw new NotFoundException<Country>();
            var city = new City
            {
                Name = dto.CityName,
                CountryId = GuidCountrId,
            };

            await _context.Cities.AddAsync(city);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<CityListDto>> GetAllCitiesAsync()
        {
            var cities = await _context.Cities
               .Select(c => new CityListDto
               {
                   Id = c.Id,
                   CityName = c.Name,
                   CountryId = c.CountryId
               })
               .ToListAsync();

            return cities;
        }

        public async Task UpdateCityAsync(string id, UpdateCityDto dto)
        {
            var cityId = Guid.Parse(id);
            var city = await _context.Cities.FindAsync(cityId)
            ?? throw new NotFoundException<City>();

            city.Name = dto.CityName;
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<CityNameDto>> GetAllCitiesByCountryIdAsync(string countryId)
        {
            var guidCountryId = Guid.Parse(countryId);
            var existCountry = await _context.Countries.FirstOrDefaultAsync(c => c.Id == guidCountryId)
                ?? throw new Exceptions.Common.NotFoundException<Country>();

            var cities = await _context.Cities
            .Where(city => city.CountryId == guidCountryId)
            .Select(city => new CityNameDto
            {
                Id = city.Id,
                CityName = city.Name,
            })
            .ToListAsync();

            return cities;
        }

        public async Task DeleteCityAsync(string cityId)
        {
            var guidCityId = Guid.Parse(cityId);
            var existCity = await _context.Cities.FindAsync(guidCityId)
                ?? throw new NotFoundException<City>();
            _context.Cities.Remove(existCity);
            await _context.SaveChangesAsync();
        }
    }
}