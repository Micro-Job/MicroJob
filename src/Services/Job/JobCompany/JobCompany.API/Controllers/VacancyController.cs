using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.Business.Services.VacancyServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacancyController(IVacancyService vacancyService) : ControllerBase
    {
        private readonly IVacancyService _vacancyService = vacancyService;

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateVacancy(CreateVacancyDto vacancyDto, ICollection<CreateNumberDto>? numberDtos)
        {
            await _vacancyService.CreateVacancyAsync(vacancyDto, numberDtos);
            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateVacancy(UpdateVacancyDto vacancyDto, ICollection<UpdateNumberDto>? numberDtos)
        {
            await _vacancyService.UpdateVacancyAsync(vacancyDto, numberDtos);
            return Ok();
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(List<string> ids)
        {
            await _vacancyService.DeleteAsync(ids);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllOwnVacanciesAsync(string? titleName, string? categoryId, string? countryId, string? cityId, bool? IsActive, decimal? minSalary, decimal? maxSalary, int skip = 1, int take = 6)
        {
            var data = await _vacancyService.GetAllOwnVacanciesAsync(titleName, categoryId, countryId, cityId, IsActive, minSalary, maxSalary, skip, take );
            return Ok(data);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetByIdVacancy(string id)
        {
            var data = await _vacancyService.GetByIdVacancyAsync(id);
            return Ok(data);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllVacanciesForApp()
        {
            return Ok(await _vacancyService.GetAllVacanciesForAppAsync());
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetVacancyByCompanyIdAsync(string companyId)
        {
            return Ok(await _vacancyService.GetVacancyByCompanyIdAsync(companyId));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllVacanciesAsync(string searchText)
        {
            return Ok(await _vacancyService.GetAllVacanciesAsync(searchText));
        }
    }
}