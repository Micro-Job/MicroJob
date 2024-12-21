using JobCompany.Business.Services.ReportServices;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobCompany.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.CompanyUser)]
    public class ReportController(IReportService reportService) : ControllerBase
    {
        /// <summary>
        /// Admin/Dashboard/ Muraciet, active vakansiya, qebul olunmus vakansiya sayi/ mohteshem 3-lu ;)
        /// </summary>

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSummary()
        {
            var data = await reportService.GetSummaryAsync();
            return Ok(data);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetRecentApplications()
        {
            var recentApplications = await reportService.GetRecentApplicationsAsync();
            return Ok(recentApplications);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetApplicationStatistics(string periodTime = "1")
        {
            var vacancyStatistics = await reportService.GetApplicationStatisticsAsync(periodTime);
            return Ok(vacancyStatistics);
        }
    }
}