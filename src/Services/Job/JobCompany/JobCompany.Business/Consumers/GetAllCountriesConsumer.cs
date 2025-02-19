using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.CountryDtos;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers
{
    public class GetAllCountriesConsumer(JobCompanyDbContext context) : IConsumer<GetAllCountriesRequest>
    {
        private readonly JobCompanyDbContext _context = context;
        public async Task Consume(ConsumeContext<GetAllCountriesRequest> context)
        {
            var countriesQuery = _context
                .Countries.Skip(Math.Max(0, (context.Message.Skip - 1) * context.Message.Take))
                .Take(context.Message.Take)
                .AsNoTracking();
            var response = await countriesQuery.Select(x => new CountryDto
            {
                Id = x.Id,
                Name = x.CountryName
            }).ToListAsync();

            var totalCount = await countriesQuery.CountAsync();
            await context.RespondAsync(new GetAllCountriesResponse { Countries = response, TotalCount = totalCount });
        }
    }
}