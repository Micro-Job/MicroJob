using JobPayment.Business.Dtos.Payment;
using JobPayment.Business.Dtos.TransactionDtos;
using JobPayment.Business.Exceptions.PaymentExceptions;
using JobPayment.Business.Services.BalanceServices;
using JobPayment.Business.Services.PriceServices;
using JobPayment.Business.Services.TransactionServices;
using JobPayment.Core.Enums;
using JobPayment.DAL.Contexts;

namespace JobPayment.Business.Services.Payment
{
    public class PaymentService(PaymentDbContext _context , IBalanceService _balanceService , IPriceService _priceService , ITransactionService _transactionService) : IPaymentService
    {
        public async Task Pay(PayDto dto)
        {
            var myBalance = await _balanceService.GetOwnBalanceAsync();
            var price = await _priceService.GetPriceByInformationTypeAsync(dto.InformationType);

            if(price.Coin > myBalance.Coin)
                throw new InsufficientBalanceException();

            await _transactionService.CreateTransactionAsync(new CreateTransactionDto
            {
                Amount = null,
                BalanceId = myBalance.Id,
                BeforeBalanceCoin = myBalance.Coin,
                Coin = price.Coin,
                InformationId = dto.InformationId,
                InformationType = dto.InformationType,
                TranzactionType = TransactionType.OutCome,
                UserId = dto.UserId 
            });

            myBalance.Coin -= price.Coin;

            await _context.SaveChangesAsync();
        }
    }
}
