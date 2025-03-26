using JobCompany.Business.Dtos.CityDtos;
using JobCompany.Business.Dtos.CountryDtos;
using JobCompany.Business.Exceptions.Common;
using JobCompany.Business.Extensions;
using JobCompany.Business.Statistics;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.CountryServices
{
    public class CountryService(JobCompanyDbContext context, ICurrentUser _user) : ICountryService
    {
        readonly JobCompanyDbContext _context = context;

        public async Task CreateCountryAsync(CountryCreateDto dto)
        {
            Country country = new();
            await _context.Countries.AddAsync(country);
            await _context.SaveChangesAsync();

            var countryTranslations = dto.Countries.Select(x => new CountryTranslation
            {
                CountryId = country.Id,
                Language = x.Language,
                Name = x.Name.Trim()
            }).ToList();

            await _context.CountryTranslations.AddRangeAsync(countryTranslations);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCountryAsync(string id)
        {
            var countryId = Guid.Parse(id);
            var country = await _context.Countries.Include(x => x.Translations).Where(x => x.Id == countryId).FirstOrDefaultAsync()
                ?? throw new NotFoundException<Country>(MessageHelper.GetMessage("NOT_FOUND"));

            var brandTranslations = country.Translations.Select(x => x).ToList();
            _context.CountryTranslations.RemoveRange(brandTranslations);
            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<CountryListDto>> GetAllCountryAsync()
        {
            var countries = await _context.Countries
                .IncludeTranslations()
            .Select(b => new CountryListDto
            {
                Id = b.Id,
                CountryName = b.GetTranslation(_user.LanguageCode,GetTranslationPropertyName.Name),
            })
            .ToListAsync();

            return countries;
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
}