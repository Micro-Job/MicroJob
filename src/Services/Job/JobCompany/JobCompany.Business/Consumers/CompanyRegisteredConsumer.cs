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
        Guid companyId = context.Message.CompanyId;

        var newCompany = new Company
        {
            Id = companyId,
            UserId = context.Message.UserId,
            CompanyName = context.Message.CompanyName,
            CompanyLogo = Path.Combine(FilePaths.image, "defaultlogo.jpg"),
            IsCompany = context.Message.IsCompany
        };

        await _dbContext.Companies.AddAsync(newCompany);

        var steps = _dbContext.ApplicationSteps.AsNoTracking();

        var statuses = steps.Select(s => new Status
        {
            CompanyId = companyId,
            StatusEnum = s.StatusEnum,
            StatusColor = s.StatusColor,
            IsVisible = s.IsVisible,
            Order = s.Order
        });

        await _dbContext.Statuses.AddRangeAsync(statuses);

        await _dbContext.SaveChangesAsync();
    }
}