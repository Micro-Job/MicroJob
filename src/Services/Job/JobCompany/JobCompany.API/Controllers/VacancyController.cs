using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.Business.Services.VacancyServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;
using System.Data;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
    public class VacancyController(IVacancyService _vacancyService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateVacancy([FromForm] CreateVacancyDto vacancyDto, [FromForm]ICollection<CreateNumberDto>? numberDtos)
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
            var data = await _vacancyService.GetAllOwnVacanciesAsync(titleName, categoryId, countryId, cityId, IsActive, minSalary, maxSalary, skip, take);
            return Ok(data);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SimilarVacancies(string vacancyId)
        {
            var data = await _vacancyService.SimilarVacanciesAsync(vacancyId);
            return Ok(data);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SimilarVacancies(string vacancyId)
        {
            var data = await vacancyService.SimilarVacanciesAsync(vacancyId);
            return Ok(data);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SimilarVacancies(string vacancyId)
        {
            var data = await vacancyService.SimilarVacanciesAsync(vacancyId);
            return Ok(data);
        }

        [AllowAnonymous]
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetByIdVacancy(string id)
        {
            var data = await _vacancyService.GetByIdVacancyAsync(id);
            return Ok(data);
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllVacanciesForApp()
        {
            return Ok(await _vacancyService.GetAllVacanciesForAppAsync());
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetVacanciesByCompanyId(string companyId)
        {
            return Ok(await _vacancyService.GetVacanciesByCompanyIdAsync(companyId));
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllVacancies(string? titleName, string? categoryId, string? countryId, string? cityId, decimal? minSalary, decimal? maxSalary, string? companyId, byte? workStyle, byte? workType, int skip = 1, int take = 9)
        {
            return Ok(await _vacancyService.GetAllVacanciesAsync(titleName, categoryId, countryId, cityId, minSalary, maxSalary, companyId, workStyle, workType, skip, take));
        }

        [HttpPost("[action]")]
        [AuthorizeRole(UserRole.SimpleUser)]
        public async Task<IActionResult> ToggleSaveVacancy(string vacancyId)
        {
            await _vacancyService.ToggleSaveVacancyAsync(vacancyId);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllSavedVacancy(int skip , int take)
        {
            return Ok(await _vacancyService.GetAllSavedVacancyAsync(skip , take));
        }
    }
}