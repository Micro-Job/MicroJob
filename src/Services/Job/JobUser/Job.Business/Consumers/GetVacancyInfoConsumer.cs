using Job.DAL.Contexts;
using MassTransit;
using SharedLibrary.Requests;

namespace Job.Business.Consumers
{
    public class GetVacancyInfoConsumer(JobDbContext context) : IConsumer<GetVacancyInfoRequest>
    {
        private readonly JobDbContext _context = context;
        public Task Consume(ConsumeContext<GetVacancyInfoRequest> context)
        {
            throw new NotImplementedException();
        }
    }
}