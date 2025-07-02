using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.Business.Exceptions.ExamExceptions;
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
    public class VacancyController(VacancyService _vacancyService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateVacancy([FromForm] CreateVacancyDto vacancyDto, [FromForm] ICollection<CreateNumberDto>? numberDtos)
        {
            await _vacancyService.CreateVacancyAsync(vacancyDto, numberDtos);
            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateVacancy([FromForm] UpdateVacancyDto vacancyDto, [FromForm] ICollection<UpdateNumberDto>? numberDtos)
        {
            await _vacancyService.UpdateVacancyAsync(vacancyDto, numberDtos);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllOwnVacancies(string? titleName, [FromQuery] List<Guid>? categoryIds, [FromQuery] List<Guid>? countryIds, [FromQuery] List<Guid>? cityIds, VacancyStatus? IsActive, decimal? minSalary, decimal? maxSalary, [FromQuery] List<byte>? workStyles, [FromQuery] List<byte>? workTypes, [FromQuery] List<Guid>? skills, int skip = 1, int take = 6)
        {
            var data = await _vacancyService.GetAllOwnVacanciesAsync(titleName, categoryIds, countryIds, cityIds, IsActive, minSalary, maxSalary, workStyles, workTypes, skills, skip, take);
            return Ok(data);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SimilarVacancies(string vacancyId)
        {
            var data = await _vacancyService.SimilarVacanciesAsync(vacancyId);
            return Ok(data);
        }

        [AllowAnonymous]
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetByIdVacancy(Guid id)
        {
            var data = await _vacancyService.GetByIdVacancyAsync(id);
            return Ok(data);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetVacancyDetails(Guid id)
        {
            var data = await _vacancyService.GetVacancyDetailsAsync(id);
            return Ok(data);
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetVacanciesByCompanyId(Guid companyId, Guid? vacancyId, int skip = 1, int take = 9)
        {
            return Ok(await _vacancyService.GetVacanciesByCompanyIdAsync(companyId, vacancyId, skip, take));
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllVacancies(string? titleName, [FromQuery] List<Guid>? categoryIds, [FromQuery] List<Guid>? countryIds, [FromQuery] List<Guid>? cityIds, decimal? minSalary, decimal? maxSalary, [FromQuery] List<Guid>? companyIds, [FromQuery] List<byte>? workStyles, [FromQuery(Name = "workTypes")] List<byte>? workTypes, [FromQuery] List<Guid> skills, int skip = 1, int take = 9)
        {
            return Ok(await _vacancyService.GetAllVacanciesAsync(titleName, categoryIds, countryIds, cityIds, minSalary, maxSalary, companyIds, workStyles, workTypes, skills, skip, take));
        }

        [HttpPost("[action]")]
        [AuthorizeRole(UserRole.SimpleUser)]
        public async Task<IActionResult> ToggleSaveVacancy(Guid vacancyId)
        {
            await _vacancyService.ToggleSaveVacancyAsync(vacancyId);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllSavedVacancy(int skip, int take, string? vacancyName)
        {
            return Ok(await _vacancyService.GetAllSavedVacancyAsync(skip, take, vacancyName));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> DeleteVacancy(Guid vacancyId)
        {
            await _vacancyService.DeleteVacancyAsync(vacancyId);
            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> TogglePauseVacancy(Guid vacancyId)
        {
            await _vacancyService.TogglePauseVacancyAsync(vacancyId);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActivateVacancy(Guid vacancyId)
        {
            await _vacancyService.ActivateVacancyAsync(vacancyId);
            return Ok();
        }
    }
}