using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.Business.Services.VacancyServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
    public class VacancyController(IVacancyService vacancyService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateVacancy([FromForm] CreateVacancyDto vacancyDto, [FromForm]ICollection<CreateNumberDto>? numberDtos)
        {
            await vacancyService.CreateVacancyAsync(vacancyDto, numberDtos);
            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateVacancy(UpdateVacancyDto vacancyDto, ICollection<UpdateNumberDto>? numberDtos)
        {
            await vacancyService.UpdateVacancyAsync(vacancyDto, numberDtos);
            return Ok();
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(List<string> ids)
        {
            await vacancyService.DeleteAsync(ids);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllOwnVacanciesAsync(string? titleName, string? categoryId, string? countryId, string? cityId, bool? IsActive, decimal? minSalary, decimal? maxSalary, int skip = 1, int take = 6)
        {
            var data = await vacancyService.GetAllOwnVacanciesAsync(titleName, categoryId, countryId, cityId, IsActive, minSalary, maxSalary, skip, take);
            return Ok(data);
        }

        [AllowAnonymous]
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetByIdVacancy(string id)
        {
            var data = await vacancyService.GetByIdVacancyAsync(id);
            return Ok(data);
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllVacanciesForApp()
        {
            return Ok(await vacancyService.GetAllVacanciesForAppAsync());
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetVacancyByCompanyIdAsync(string companyId)
        {
            return Ok(await vacancyService.GetVacancyByCompanyIdAsync(companyId));
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllVacanciesAsync(string? titleName, string? categoryId, string? countryId, string? cityId, decimal? minSalary, decimal? maxSalary, int skip = 1, int take = 9)
        {
            return Ok(await vacancyService.GetAllVacanciesAsync(titleName, categoryId, countryId, cityId, minSalary, maxSalary, skip, take));
        }

        [HttpPost("[action]")]
        [AuthorizeRole(UserRole.SimpleUser)]
        public async Task<IActionResult> ToggleSaveVacancy(string vacancyId)
        {
            await vacancyService.ToggleSaveVacancyAsync(vacancyId);
            return Ok();
        }
    }
}