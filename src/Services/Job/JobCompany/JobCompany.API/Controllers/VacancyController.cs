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
        public async Task<IActionResult> GetAllVacancies()
        {
            var data = await _vacancyService.GetAllVacanciesAsync();
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
    }
}