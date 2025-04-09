using JobPayment.Business.Services.TransactionServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobPayment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController(ITransactionService _transactionService) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetOwnTransactions(string? startDate, string? endDate, byte? transactionStatus, byte? informationType, byte? transactionType, int skip = 1, int take = 7)
        {
            return Ok(await _transactionService.GetOwnTransactionsAsync(startDate,endDate,transactionStatus,informationType,transactionType,skip,take));
        }
    }
}
