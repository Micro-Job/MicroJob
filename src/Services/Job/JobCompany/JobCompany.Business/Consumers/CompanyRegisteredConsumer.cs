using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Events;
using SharedLibrary.Statics;

namespace JobCompany.Business.Consumers;

public class CompanyRegisteredConsumer(JobCompanyDbContext _dbContext) : IConsumer<CompanyRegisteredEvent>
{
    public async Task Consume(ConsumeContext<CompanyRegisteredEvent> context)
    {
        var message = context.Message;

        var newCompany = new Company
        {
            Id = message.CompanyId,
            UserId = message.UserId,
            CompanyName = message.CompanyName,
            CompanyLogo = Path.Combine(FilePaths.image, "defaultlogo.jpg"),
            IsCompany = message.IsCompany,
            VOEN = message.IsCompany ? message.VOEN : null,
            Email = message.Email,
            MainPhoneNumber = message.MainPhoneNumber
        };

        await _dbContext.Companies.AddAsync(newCompany);

        await _dbContext.CompanyNumbers.AddAsync(new CompanyNumber
        {
            CompanyId = message.CompanyId,
            Number = message.MainPhoneNumber
        });

        var steps = await _dbContext.ApplicationSteps.ToListAsync();

        var statuses = steps.Select(s => new Status
        {
            CompanyId = message.CompanyId,
            StatusEnum = s.StatusEnum,
            StatusColor = s.StatusColor,
            IsVisible = s.IsVisible,
            Order = s.Order
        });

        await _dbContext.Statuses.AddRangeAsync(statuses);

        await _dbContext.SaveChangesAsync();
    }
}