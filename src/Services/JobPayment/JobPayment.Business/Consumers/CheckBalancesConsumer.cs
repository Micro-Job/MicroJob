using JobPayment.Business.Services.BalanceServices;
using JobPayment.Business.Services.PriceServices;
using JobPayment.Core.Entities;
using JobPayment.DAL.Contexts;
using MassTransit;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Consumers
{
    public class CheckBalancesConsumer(BalanceService _balanceService, PriceService _priceService) : IConsumer<CheckBalancesRequest>
    {
        public async Task Consume(ConsumeContext<CheckBalancesRequest> context)
        {
            Service service = await _priceService.GetPriceByInformationTypeAsync(context.Message.InformationType);

            List<Balance> userBalances = await _balanceService.GetUsersBalancesAsync(context.Message.UserIds);

            ICollection<BalancesResponseDto> balancesResponse = userBalances.Select(x => new BalancesResponseDto
            {
                UserId = x.UserId,
                Coin = x.Coin,
                HasEnoughBalance = service.Coin <= x.Coin
            }).ToList();

            await context.RespondAsync(new CheckBalancesResponse
            {
                UserBalanceInformations = balancesResponse
            });
        }
    }
}
