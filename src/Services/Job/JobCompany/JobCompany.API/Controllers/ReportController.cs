﻿using JobCompany.Business.Services.ReportServices;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController(IReportService reportService) : ControllerBase
    {
        private readonly IReportService _reportService = reportService;
        /// <summary>
        /// Admin/Dashboard/ Muraciet, active vakansiya, qebul olunmus vakansiya sayi/ mohteshem 3-lu)))
        /// </summary>
        [HttpGet("[action]")]
        public async Task<IActionResult> GetSummary()
        {
            var data = await _reportService.GetSummaryAsync();
            return Ok(data);
        }
    }
}