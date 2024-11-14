using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using SharedLibrary.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Consumers
{
    internal class CompanyRegisteredConsumer(JobCompanyDbContext _context) : IConsumer<CompanyRegisteredEvent>
    {
        public async Task Consume(ConsumeContext<CompanyRegisteredEvent> context)
        {
            var newCompany = new Company
            {
                Id = context.Message.CompanyId,
                CompanyInformation = context.Message.CompanyInformation,
            };

            await _context.Companies.AddAsync(newCompany);
            await _context.SaveChangesAsync();
        }
    }
}
