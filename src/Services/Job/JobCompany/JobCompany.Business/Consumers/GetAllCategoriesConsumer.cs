using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.CategoryDtos;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers
{
    public class GetAllCategoriesConsumer(JobCompanyDbContext context) : IConsumer<GetAllCategoriesRequest>
    {
        private readonly JobCompanyDbContext _context = context;
        public async Task Consume(ConsumeContext<GetAllCategoriesRequest> context)
        {
            var categoriesQuery = _context
                .Categories.Skip(Math.Max(0, (context.Message.Skip - 1) * context.Message.Take))
                .Take(context.Message.Take)
                .AsNoTracking();
            var response = await categoriesQuery.Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.CategoryName,
            }).ToListAsync();

            var totalCount = await categoriesQuery.CountAsync();
            await context.RespondAsync(new GetAllCategoriesResponse { Categories = response, TotalCount = totalCount });
        }
    }
}