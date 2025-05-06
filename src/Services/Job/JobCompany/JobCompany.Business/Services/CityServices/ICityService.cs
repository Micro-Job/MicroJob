using JobCompany.Business.Dtos.CityDtos;

namespace JobCompany.Business.Services.CityServices;

public interface ICityService
{
    Task CreateCityAsync(CreateCityDto dto);
    Task UpdateCityAsync(List<UpdateCityDto> cities);
    Task<ICollection<CityListDto>> GetAllCitiesAsync();
    Task<ICollection<CityNameDto>> GetAllCitiesByCountryIdAsync(string countryId);
    Task DeleteCityAsync(string cityId);
}