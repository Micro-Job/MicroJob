using JobPayment.Business.Services.BalanceServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

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

        [HttpGet("[action]")]
        public async Task<IActionResult> GetOwnBalance()
        {
            return Ok(await _balanceService.GetOwnBalanceAsync());
        }
    }
}
