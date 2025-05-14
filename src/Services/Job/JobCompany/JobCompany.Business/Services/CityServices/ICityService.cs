using JobCompany.Business.Dtos.CityDtos;
using JobCompany.Business.Dtos.Common;

namespace JobCompany.Business.Services.CityServices;

public interface ICityService
{
    Task CreateCityAsync(CreateCityDto dto);
    Task UpdateCityAsync(List<UpdateCityDto> cities);
    Task<ICollection<CityListDto>> GetAllCitiesAsync();
    Task<ICollection<CityNameDto>> GetAllCitiesByCountryIdAsync(string countryId);
    Task<DataListDto<CityNameDto>> GetAllCitiesByCountryIdsAsync(List<string> countryIds, string? name, int skip, int take);
    Task DeleteCityAsync(string cityId);
}