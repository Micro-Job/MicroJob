using Job.Business.Services.Vacancy;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserVacancyController(IVacancyService _vacancyService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> ToggleSaveVacancy(string vacancyId)
        {
            await _vacancyService.ToggleSaveVacancyAsync(vacancyId);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllSavedVacancy()
        {
            return Ok(await _vacancyService.GetAllSavedVacancyAsync());
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCompanies()
        {
            return Ok(await _vacancyService.GetAllCompaniesAsync());
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllUserVacancies()
        {
            return Ok(await _vacancyService.GetAllUserVacanciesAsync());
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetVacancyInfo(Guid vacancyId)
        {
            var data = await _vacancyService.GetVacancyInfoAsync(vacancyId);
            return Ok(data);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllVacanciesAsync(string titleName, string categoryId, string countryId, string cityId, bool? isActive, decimal? minSalary, decimal? maxSalary, int skip = 1, int take = 6)
        {
            return Ok(await _vacancyService.GetAllVacanciesAsync(titleName, categoryId, countryId, cityId, isActive, minSalary, maxSalary, skip, take));
        }

    }
}