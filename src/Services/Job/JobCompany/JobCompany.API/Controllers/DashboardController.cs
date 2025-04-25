using JobCompany.Business.Services.DashboardService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobCompany.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController(DashboardService _dashboardService) : ControllerBase
    {

    }
}
