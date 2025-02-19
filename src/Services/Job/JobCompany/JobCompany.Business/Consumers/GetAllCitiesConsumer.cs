using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.CityDtos;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace JobCompany.Business.Consumers
{
    public class GetAllCitiesConsumer(JobCompanyDbContext context) : IConsumer<GetAllCitiesRequest>
    {
        private readonly JobCompanyDbContext _context = context;
        public async Task Consume(ConsumeContext<GetAllCitiesRequest> context)
        {
            var citiesQuery = _context
                .Cities.Where(x => x.CountryId == context.Message.CountryId)
                       .Skip(Math.Max(0, (context.Message.Skip - 1) * context.Message.Take))
                       .Take(context.Message.Take)
                       .AsNoTracking();
            var response = await citiesQuery.Select(x => new CityDto
            {
                Id = x.Id,
                Name = x.CityName
            }).ToListAsync();

            var totalCount = await citiesQuery.CountAsync();
            await context.RespondAsync(new GetAllCitiesResponse { Cities = response, TotalCount = totalCount });
        }
    }
}