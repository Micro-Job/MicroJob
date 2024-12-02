using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;

namespace JobCompany.Business.Consumers
{
    public class UserApplicationConsumer(JobCompanyDbContext _companyDb): IConsumer<UserApplicationEvent>
    {
        public async Task Consume(ConsumeContext<UserApplicationEvent> context)
        {
            var status = await _companyDb.Statuses.FirstOrDefaultAsync(s => s.Order == 1)
            ?? throw new SharedLibrary.Exceptions.NotFoundException<Status>();
            var newApp = new Application
            {
                UserId = context.Message.UserId,
                VacancyId = context.Message.VacancyId,
                IsActive = true,
                StatusId = status.Id
            };
            await _companyDb.Applications.AddAsync(newApp);
            await _companyDb.SaveChangesAsync();
        }
    }
}