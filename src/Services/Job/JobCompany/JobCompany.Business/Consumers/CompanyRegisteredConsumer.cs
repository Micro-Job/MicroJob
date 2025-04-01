using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using SharedLibrary.Enums;
using SharedLibrary.Events;

namespace JobCompany.Business.Consumers
{
    public class CompanyRegisteredConsumer(JobCompanyDbContext _context) : IConsumer<CompanyRegisteredEvent>
    {
        public async Task Consume(ConsumeContext<CompanyRegisteredEvent> context)
        {
            Guid companyId = context.Message.CompanyId;

            var newCompany = new Company
            {
                Id = companyId,
                UserId = context.Message.UserId,
                CompanyName = context.Message.CompanyName,
                CompanyLogo = context.Message.CompanyLogo,
                IsCompany = context.Message.IsCompany
            };

            await _context.Companies.AddAsync(newCompany);

            var statuses = new List<Status>
            {
                new Status
                {
                    StatusColor = "#28a745", // Yeşil
                    IsVisible = true,
                    Order = 1,
                    StatusEnum = StatusEnum.Pending,
                    CompanyId = companyId
                },
                new Status
                {
                    StatusColor = "#ffc107", // Sarı
                    IsVisible = true,
                    Order = 2,
                    StatusEnum = StatusEnum.Interview,
                    CompanyId = companyId
                },
                new Status
                {
                    StatusColor = "#17a2b8", // Mavi
                    IsVisible = true,
                    Order = 3,
                    StatusEnum = StatusEnum.Accepted,
                    CompanyId = companyId
                },
                new Status
                {
                    StatusColor = "#dc3545", // Kırmızı
                    IsVisible = true,
                    Order = 4,
                    StatusEnum = StatusEnum.Rejected,
                    CompanyId = companyId
                },
            };

            await _context.Statuses.AddRangeAsync(statuses);

            await _context.SaveChangesAsync();
        }
    }
}