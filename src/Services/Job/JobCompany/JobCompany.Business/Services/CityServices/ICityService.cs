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
    }
}