using JobPayment.Business.Dtos.Common;
using JobPayment.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobPayment.Business.Consumers;

public class GetUserTransactionsConsumer(PaymentDbContext _paymentDb) : IConsumer<GetUserTransactionsRequest>
{
    public async Task Consume(ConsumeContext<GetUserTransactionsRequest> context)
    {
        var query = _paymentDb.Transactions
            .Where(x => x.UserId == context.Message.UserId)
            .OrderByDescending(x => x.CreatedDate)
            .AsNoTracking()
            .AsQueryable();

        var count = await query.CountAsync();

        var transactions = await query
            .Skip(Math.Max(0, context.Message.PageIndex - 1) * context.Message.PageSize)
            .Take(context.Message.PageSize)
            .Select(x => new GetUserTransactionsResponse
            {
                Id = x.Id,
                Coin = x.Coin,
                CreatedDate = x.CreatedDate,
                InformationId = x.InformationId,
                InformationType = (byte)x.InformationType,
                TransactionStatus = (byte)x.TransactionStatus,
                TransactionType = (byte)x.TranzactionType
            })
        .ToListAsync();

        await context.RespondAsync(new DataListDto<GetUserTransactionsResponse>
        {
            Datas = transactions,
            TotalCount = count,
            TotalPage = (int)Math.Ceiling((double)count / context.Message.PageSize)
        });
    }
}
