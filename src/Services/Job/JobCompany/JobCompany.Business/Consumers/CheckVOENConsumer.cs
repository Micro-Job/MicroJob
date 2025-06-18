using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Consumers
{
    public class CheckVOENConsumer(JobCompanyDbContext _context) : IConsumer<CheckVoenRequest>
    {
        public async Task Consume(ConsumeContext<CheckVoenRequest> context)
        {
            bool IsExist = await _context.Companies.AnyAsync(x => x.VOEN == context.Message.VOEN);

            await context.RespondAsync(new CheckVoenResponse
            {
                IsExist = IsExist
            });
        }
    }
}
