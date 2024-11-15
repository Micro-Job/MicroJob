using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using SharedLibrary.Events;

namespace JobCompany.Business.Consumers
{
    internal class CompanyRegisteredConsumer(JobCompanyDbContext _context) : IConsumer<CompanyRegisteredEvent>
    {
        public async Task Consume(ConsumeContext<CompanyRegisteredEvent> context)
        {
            var newCompany = new Company
            {
                Id = context.Message.CompanyId,
                UserId = context.Message.UserId,
                CompanyName = context.Message.CompanyName,
            };

            await _context.Companies.AddAsync(newCompany);
            await _context.SaveChangesAsync();
        }
    }
}