using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.CountryDtos;

namespace JobCompany.Business.Services.CountryServices
{
    public interface ICountryService
    {
        Task CreateCountryAsync(CreateCountryDto dto);
        Task UpdateCountryAsync(string id, UpdateCountryDto dto);
        Task<ICollection<CountryListDto>> GetAllCountryAsync();
        Task DeleteCountryAsync(string id);
    }
}