using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.CountryDtos;

namespace JobCompany.Business.Services.CountryServices;

public interface ICountryService
{
    Task CreateCountryAsync(CountryCreateDto dto);
    Task UpdateCountryAsync(List<CountryUpdateDto> countries);
    Task<ICollection<CountryListDto>> GetAllCountryAsync();
    Task<DataListDto<CountryListDto>> GetPagedCountriesAsync(string? name, int skip, int take);
    Task DeleteCountryAsync(string id);
}