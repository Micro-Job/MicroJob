using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.CityDtos;
using JobCompany.Business.Services.CityServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController : ControllerBase
    {
        readonly ICityService _service;

        public CityController(ICityService service)
        {
            _service = service;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> CreateCityAsync(CreateCityDto dto)
        {
            await _service.CreateCityAsync(dto);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateCityAsync(string id, UpdateCityDto dto)
        {
            await _service.UpdateCityAsync(id, dto);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCitiesAsync()
        {
            await _service.GetAllCitiesAsync();
            return Ok();
        }
    }
}