using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.CityDtos;

namespace JobCompany.Business.Services.CityServices
{
    public interface ICityService
    {
        Task CreateCityAsync(CreateCityDto dto);
        Task UpdateCityAsync(string id, UpdateCityDto dto);
        Task<ICollection<CityListDto>> GetAllCitiesAsync();
        Task<ICollection<CityNameDto>> GetAllCitiesByCountryIdAsync(string countryId);
        Task DeleteCityAsync(string cityId);
    }
}