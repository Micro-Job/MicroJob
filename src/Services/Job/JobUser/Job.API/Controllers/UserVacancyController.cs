﻿using Microsoft.AspNetCore.Http;
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

    }
}
