using JobPayment.Business.Services.TransactionServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobPayment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController(TransactionService _transactionService) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetOwnTransactions(string? startDate, string? endDate, byte? transactionStatus, byte? informationType, byte? transactionType, int skip = 1, int take = 7)
        {
            return Ok(await _transactionService.GetOwnTransactionsAsync(startDate,endDate,transactionStatus,informationType,transactionType,skip,take));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetTransactionDetail(Guid transactionId)
        {
            return Ok(await _transactionService.GetTransactionDetailAsync(transactionId));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllTransactionsByUserId(Guid userId, string? startDate, string? endDate, byte? transactionStatus, byte? informationType, byte? transactionType, int skip, int take)
        {
            return Ok(await _transactionService.GetAllTransactionsByUserIdAsync(userId , startDate , endDate , transactionStatus , informationType , transactionType , skip , take));
        }

        [HttpGet("[action]")]
        [AuthorizeRole(UserRole.Admin, UserRole.Operator)]
        public async Task<IActionResult> GetAllTransactions(string? searchTerm, string? startDate, string? endDate, byte? transactionStatus, byte? informationType, byte? transactionType, int skip = 1, int take = 7)
        {
            return Ok(await _transactionService.GetAllTransactionsAsync(searchTerm, startDate, endDate, transactionStatus, informationType, transactionType, skip, take));
        }
    }
}
