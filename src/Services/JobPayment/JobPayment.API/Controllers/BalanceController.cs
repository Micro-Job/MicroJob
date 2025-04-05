using JobPayment.Business.Services.BalanceSer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobPayment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BalanceController(IBalanceService _balanceService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> IncreaseBalance(string packetId)
        {
            await _balanceService.IncreaseBalanceAsync(packetId);
            return Ok();
        }
    }
}
