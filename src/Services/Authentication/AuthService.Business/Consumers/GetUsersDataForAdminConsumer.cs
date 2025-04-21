using AuthService.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace AuthService.Business.Consumers;

public class GetUsersDataForAdminConsumer(AppDbContext _appDbContext) : IConsumer<GetUsersDataForAdminRequest>
{
    public async Task Consume(ConsumeContext<GetUsersDataForAdminRequest> context)
    {
        var query = _appDbContext.Users.Where(u => u.UserRole == context.Message.UserRole);

        if (!string.IsNullOrEmpty(context.Message.SearchTerm))
        {
            var searchTerm = context.Message.SearchTerm.Trim().ToLower();

            query = query.Where(u => (u.FirstName + " " + u.LastName).ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();

        query = query
            .Skip((context.Message.PageIndex - 1) * context.Message.PageSize)
            .Take(context.Message.PageSize);

        var users = await query
            .Select(u => new GetUsersDataForAdminResponse
            {
                UserId = u.Id,
                FullName = $"{u.FirstName} {u.LastName}",
                Email = u.Email,
                MainPhoneNumber = u.MainPhoneNumber
            })
            .ToListAsync();

        var response = new PaginatedResponse<GetUsersDataForAdminResponse>
        {
            Datas = users,
            TotalCount = totalCount
        };

        await context.RespondAsync(response);
    }
}
