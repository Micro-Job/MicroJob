using JobCompany.Business.Dtos.CityDtos;
using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Extensions;
using JobCompany.Business.Statistics;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Exceptions;
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
        var cities = await _context.Cities.Include(x=> x.Translations)
        .AsNoTracking()
        .Select(b => new CityListDto
        {
            Id = b.Id,
            CityName = b.Translations.GetTranslation(_user.LanguageCode, GetTranslationPropertyName.Name),
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

    /// <summary>
    /// Vakansiyalar siyahısının yan panelindəki filtr bölməsində istifadə olunan şəhərlərin siyahısını gətirir.
    /// Verilmiş ölkə ID-lərinə aid şəhərləri ad üzrə filtirləyərək və səhifələnmiş şəkildə qaytarır
    /// </summary>
    public async Task<DataListDto<CityNameDto>> GetAllCitiesByCountryIdsAsync(List<string> countryIds, string? name, int skip, int take)
    {
        var guidCountryIds = countryIds.Select(Guid.Parse);

        var query = _context.Cities.AsNoTracking().Where(c => guidCountryIds.Contains(c.CountryId));

        if (!string.IsNullOrWhiteSpace(name))
        {
            name = name.Trim();
            query = query.Where(c => c.Translations.Any(t => t.Language == _user.LanguageCode && t.Name.ToLower().Contains(name)));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip(skip)
            .Take(take)
            .Select(c => new CityNameDto
            {
                Id = c.Id,
                CityName = c.Translations.Where(t => t.Language == _user.LanguageCode).Select(t => t.Name).FirstOrDefault()!,
            })
            .OrderBy(c => c.CityName)
            .ToListAsync();

        return new DataListDto<CityNameDto> { Datas = items, TotalCount = totalCount };
    }

    public async Task DeleteCityAsync(string cityId)
    {
        var cityGuid = Guid.Parse(cityId);

        var city = await _context.Cities.FindAsync(cityGuid) ?? throw new NotFoundException();

        _context.Cities.Remove(city);

        await _context.SaveChangesAsync();
    }
}