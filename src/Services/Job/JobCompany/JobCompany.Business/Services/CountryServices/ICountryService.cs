using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.CountryDtos;

namespace JobCompany.Business.Services.CountryServices
{
    public interface ICountryService
    {
        Task CreateCountryAsync(string countryName);
        Task UpdateCountryAsync(string id, string? countryName);
        Task<ICollection<CountryListDto>> GetAllCountryAsync();
        Task DeleteCountryAsync(string id);
    }
}