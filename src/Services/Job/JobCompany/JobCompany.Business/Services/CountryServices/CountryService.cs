using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.CountryDtos;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Exceptions;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.CountryServices;

public class CountryService(JobCompanyDbContext _context, ICurrentUser _user) 
{
    public async Task CreateCountryAsync(CountryCreateDto dto)
    {
        var country = new Country
        {
            Translations = dto.Countries.Select(x => new CountryTranslation
            {
                Language = x.Language,
                Name = x.Name.Trim()
            }).ToList()
        };

        await _context.Countries.AddAsync(country);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCountryAsync(string id)
    {
        var countryId = Guid.Parse(id);
        var country = await _context.Countries.FindAsync(countryId) ?? throw new NotFoundException();

        _context.Countries.Remove(country);
        await _context.SaveChangesAsync();
    }

    public async Task<ICollection<CountryListDto>> GetAllCountryAsync()
    {
        var countries = await _context.Countries
        .AsNoTracking()
        .Select(c => new CountryListDto
        {
            Id = c.Id,
            CountryName = c.Translations.Where(t => t.Language == _user.LanguageCode).Select(t => t.Name).FirstOrDefault(),
        })
        .OrderBy(c => c.CountryName)
        .ToListAsync();

        return countries;
    }

    /// <summary>
    /// Vakansiyalar siyahısının yan panelindəki filtr bölməsində istifadə olunan ölkələrin siyahısını gətirir.
    /// </summary>
    public async Task<DataListDto<CountryListDto>> GetPagedCountriesAsync(string? name, int skip, int take)
    {
        var query = _context.Countries.AsNoTracking();

        if (!string.IsNullOrEmpty(name))
        {
            name = name.Trim();
            query = query.Where(c => c.Translations.Any(t => t.Name.ToLower().Contains(name)));
        }

        var totalCount = await query.CountAsync();

        var countries = await query
            .Skip((skip - 1) * take)
            .Take(take)
            .Select(c => new CountryListDto
            {
                Id = c.Id,
                CountryName = c.Translations.Where(t => t.Language == _user.LanguageCode).Select(t => t.Name).FirstOrDefault()!,
            })
            .OrderBy(c => c.CountryName)
            .ToListAsync();

        return new DataListDto<CountryListDto> { Datas = countries, TotalCount = totalCount };
    }

    public async Task UpdateCountryAsync(List<CountryUpdateDto> countries)
    {
        var countryTranslations = await _context.CountryTranslations
            .Where(x => countries.Select(b => b.Id).Contains(x.Id))
            .ToListAsync();

        foreach (var translation in countryTranslations)
        {
            var category = countries.FirstOrDefault(b => b.Id == translation.Id);
            if (category != null)
            {
                translation.Name = category.Name;
            }
        }
        await _context.SaveChangesAsync();
    }
}