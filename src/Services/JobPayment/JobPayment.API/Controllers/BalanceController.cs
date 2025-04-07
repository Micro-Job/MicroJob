using JobPayment.Business.Services.BalanceSer;
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
        public async Task<IActionResult> IncreaseBalance(string packetId , int number)
        {
            await _balanceService.IncreaseBalanceAsync(packetId , number);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetOwnBalance()
        {
            return Ok(await _balanceService.GetOwnBalanceAsync());
        }
    }
}
