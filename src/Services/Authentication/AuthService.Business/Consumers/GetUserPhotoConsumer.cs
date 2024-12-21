using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Requests;
using Shared.Responses;

namespace AuthService.Business.Consumers
{
    public class GetUserPhotoConsumer : IConsumer<GetResumeUserPhotoRequest>
    {
        private readonly AppDbContext _context;

        public GetUserPhotoConsumer(AppDbContext context)
        {
            _context = context;
        }
        public async Task Consume(ConsumeContext<GetResumeUserPhotoRequest> context)
        {
                var user = await _context.Users
                .Where(x => x.Id == context.Message.UserId)
                .Select(x => new
                {
                    x.Image
                })
                .FirstOrDefaultAsync();

                if (user is null)
                {
                    await context.RespondAsync<GetResumeUserPhotoResponse>(null);
                    return;
                }

                await context.RespondAsync(new GetResumeUserPhotoResponse
                {
                    ProfileImage = user.Image
                });
        }
    }
}