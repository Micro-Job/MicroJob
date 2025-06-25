using JobCompany.Business.Services.ReportServices;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.CompanyUser, UserRole.EmployeeUser)]
    public class ReportController(ReportService reportService) : ControllerBase
    {
        /// <summary>
        /// Admin/Dashboard/ Muraciet, active vakansiya, qebul olunmus vakansiya sayi/ mohteshem 3-lu ;)
        /// </summary>

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSummary()
        {
            return Ok(await reportService.GetSummaryAsync());
        }

        //[HttpGet("[action]")]
        //public async Task<IActionResult> GetRecentApplications()
        //{
        //    return Ok(await reportService.GetRecentApplicationsAsync());
        //}

        [HttpGet("[action]")]
        public async Task<IActionResult> GetApplicationStatistics(byte periodTime = 1)
        {
            return Ok(await reportService.GetApplicationStatisticsAsync(periodTime));
        }
    }
}