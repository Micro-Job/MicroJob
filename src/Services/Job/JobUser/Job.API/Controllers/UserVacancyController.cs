﻿using Job.Business.Services.Vacancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserVacancyController(IVacancyService _vacancyService) : ControllerBase
    {
        [HttpPost("[action]")]
        [AuthorizeRole(UserRole.SimpleUser)]
        public async Task<IActionResult> ToggleSaveVacancy(string vacancyId)
        {
            await _vacancyService.ToggleSaveVacancyAsync(vacancyId);
            return Ok();
        }

        [HttpGet("[action]")]
        [AuthorizeRole(UserRole.SimpleUser)]
        public async Task<IActionResult> GetAllSavedVacancy()
        {
            return Ok(await _vacancyService.GetAllSavedVacancyAsync());
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCompanies(string? searchTerm)
        {
            return Ok(await _vacancyService.GetAllCompaniesAsync(searchTerm));
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> GetVacancyInfo(Guid vacancyId)
        {
            var data = await _vacancyService.GetVacancyInfoAsync(vacancyId);
            return Ok(data);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllVacancies(string? titleName, string? categoryId, string? countryId, string? cityId, bool? isActive, decimal? minSalary, decimal? maxSalary, int skip = 1, int take = 6)
        {
            return Ok(await _vacancyService.GetAllVacanciesAsync(titleName, categoryId, countryId, cityId, isActive, minSalary, maxSalary, skip, take));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SimilarVacancies(string vacancyId)
        {
            var data = await _vacancyService.SimilarVacanciesAsync(vacancyId);
            return Ok(data);
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> GetOtherVacanciesByCompanyAsync(string companyId, string? currentVacancyId)
        {
            return Ok(await _vacancyService.GetOtherVacanciesByCompanyAsync(companyId, currentVacancyId));
        }
    }
}