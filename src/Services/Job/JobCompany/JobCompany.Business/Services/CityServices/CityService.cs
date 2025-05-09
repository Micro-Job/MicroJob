using JobCompany.Business.Dtos.CategoryDtos;
using JobCompany.Business.Dtos.CityDtos;
using JobCompany.Business.Exceptions.Common;
using JobCompany.Business.Extensions;
using JobCompany.Business.Statistics;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.CityServices;

public class CityService(JobCompanyDbContext _context, ICurrentUser _user) : ICityService
{
    public async Task CreateCityAsync(CreateCityDto dto)
    {
        var city = new City
        {
            CountryId = dto.CountryId,
            Translations = dto.Cities.Select(x => new CityTranslation
            {
                Language = x.language,
                Name = x.Name.Trim()
            }).ToList()
        };

        await _context.Cities.AddAsync(city);
        await _context.SaveChangesAsync();
    }

    public async Task<ICollection<CityListDto>> GetAllCitiesAsync()
    {
        var cities = await _context.Cities
        .AsNoTracking()
        .IncludeTranslations()
        .Select(b => new CityListDto
        {
            Id = b.Id,
            CityName = b.GetTranslation(_user.LanguageCode, GetTranslationPropertyName.Name),
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
            .AsNoTracking()
            .Where(city => city.CountryId == guidCountryId)
            .Select(c => new CityNameDto
            {
                Id = c.Id,
                CityName = c.Translations.Where(t => t.Language == _user.LanguageCode).Select(t => t.Name).FirstOrDefault(),
            })
            .OrderBy(c => c.CityName)
            .ToListAsync();

        return cities;
    }

    public async Task DeleteCityAsync(string cityId)
    {
        var cityGuid = Guid.Parse(cityId);

        var city = await _context.Cities.FindAsync(cityGuid) ?? throw new NotFoundException<City>();

        _context.Cities.Remove(city);

        await _context.SaveChangesAsync();
    }
}