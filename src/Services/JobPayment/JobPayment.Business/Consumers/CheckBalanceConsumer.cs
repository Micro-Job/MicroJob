using JobPayment.Business.Dtos.BalanceDtos;
using JobPayment.Business.Services.BalanceServices;
using JobPayment.Business.Services.PriceServices;
using JobPayment.Core.Entities;
using MassTransit;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobPayment.Business.Consumers
{
    public class CheckBalanceConsumer(IBalanceService _balanceService , IPriceService _priceService) : IConsumer<CheckBalanceRequest>
    {
        public async Task Consume(ConsumeContext<CheckBalanceRequest> context)
        {
            BalanceDto userBalance = await _balanceService.GetUserBalanceByIdAsync(context.Message.UserId);

            Service service = await _priceService.GetPriceByInformationTypeAsync(context.Message.InformationType);

            await context.RespondAsync(new CheckBalanceResponse
            {
                Coin = userBalance.Coin,
                HasBalance = userBalance.Coin > 0,
                HasEnoughBalance = service.Coin <= userBalance.Coin
            });
        }
    }
}
