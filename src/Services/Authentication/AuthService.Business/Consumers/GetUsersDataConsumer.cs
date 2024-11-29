using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.DAL.Contexts;
using MassTransit;
using Shared.Requests;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Business.Consumers
{
    public class GetUsersDataConsumer : IConsumer<GetUsersDataRequest>
    {
        private readonly AppDbContext _context;

        public GetUsersDataConsumer(AppDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<GetUsersDataRequest> context)
        {
            var userIds = context.Message.UserIds;

            var users = await _context.Users
                .Where(x => userIds.Contains(x.Id)) 
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.Image,
                })
                .ToListAsync(); 

            await context.RespondAsync(new
            {
                Users = users
            });
        }
    }
}
