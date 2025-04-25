using JobPayment.Core.Entities;
using JobPayment.DAL.Contexts;
using MassTransit;
using SharedLibrary.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Consumers
{
    public class CreateBalanceConsumer(PaymentDbContext _context) : IConsumer<CreateBalanceEvent>
    {
        public async Task Consume(ConsumeContext<CreateBalanceEvent> context)
        {
            var message = context.Message;

            await _context.Users.AddAsync(new User
            {
                Id = message.UserId,
                FirstName = message.FirstName,
                LastName = message.LastName
            });

            await _context.Balances.AddAsync(new Balance
            {
                UserId = message.UserId,
                Coin = 0,
                BonusCoin = 0,
            });
            await _context.SaveChangesAsync();
        }
    }
}
