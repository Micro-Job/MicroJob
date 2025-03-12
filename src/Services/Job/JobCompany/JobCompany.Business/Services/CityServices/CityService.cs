using JobCompany.Business.Dtos.CategoryDtos;
using JobCompany.Business.Dtos.CityDtos;
using JobCompany.Business.Exceptions.Common;
using JobCompany.Business.Extensions;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.CityServices
{
    public class CityService(JobCompanyDbContext context, ICurrentUser _user) : ICityService
    {
        readonly JobCompanyDbContext _context = context;

        public async Task CreateCityAsync(CreateCityDto dto)
        {
            City city = new City
            {
                CountryId = dto.CountryId  
            };
            await _context.Cities.AddAsync(city); 
            await _context.SaveChangesAsync();

            var cityTranslations = dto.Cities.Select(x => new CityTranslation
            {
                CityId = city.Id,
                Language = x.language,
                Name = x.Name.Trim()
            }).ToList();

            await _context.CityTranslations.AddRangeAsync(cityTranslations);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<CityListDto>> GetAllCitiesAsync()
        {
            var cities = await _context.Cities
                .IncludeTranslations()
            .Select(b => new CityListDto
            {
                Id = b.Id,
                CityName = b.GetTranslation(_user.LanguageCode),
                CountryId = b.CountryId,
            })
            .ToListAsync();

            return cities;
        }

        public async Task UpdateCityAsync(List<UpdateCityDto> cities)
        {
            var cityTranslations = await _context.CityTranslations
            .Where(x => cities.Select(b => b.Id).Contains(x.Id))
            .ToListAsync();

            foreach (var translation in cityTranslations)
            {
                var city = cities.FirstOrDefault(b => b.Id == translation.Id);
                if (city != null)
                {
                    translation.Name = city.Name;
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<CityNameDto>> GetAllCitiesByCountryIdAsync(string countryId)
        {
            var guidCountryId = Guid.Parse(countryId);

            var cities = await _context.Cities
                .Where(city => city.CountryId == guidCountryId)
                .Select(b => new CityNameDto
                {
                    Id = b.Id,
                    CityName = b.GetTranslation(_user.LanguageCode),
                })
                .ToListAsync();

            return cities;
        }

        public async Task DeleteCityAsync(string cityId)
        {
            var cityGuid = Guid.Parse(cityId);
            var city = await _context.Cities.Include(x => x.Translations).Where(x => x.Id == cityGuid).FirstOrDefaultAsync();

            var cityTranslations = city.Translations.Select(x => x).ToList();
            _context.CityTranslations.RemoveRange(cityTranslations);
            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
        }
    }
}