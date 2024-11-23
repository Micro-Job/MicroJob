using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Business.Dtos.CountryDtos;
using JobCompany.Business.Services.CountryServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        readonly ICountryService _service;

        public CountryController(ICountryService service)
        {
            _service = service;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> CreateCountryAsync(CreateCountryDto dto)
        {
            await _service.CreateCountryAsync(dto);
            return Ok();
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> UpdateCountryAsync(string id, UpdateCountryDto dto)
        {
            await _service.UpdateCountryAsync(id, dto);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCountriesAsync()
        {
            await _service.GetAllCountryAsync();
            return Ok();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteCountryAsync(string id)
        {
            await _service.DeleteCountryAsync(id);
            return Ok();
        }
    }
}