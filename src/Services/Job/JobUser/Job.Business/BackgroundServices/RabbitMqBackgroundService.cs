using Job.Business.Consumers;
using Job.DAL.Contexts;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Job.Business.BackgroundServices
{
    public class RabbitMqBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public RabbitMqBackgroundService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var connectionString = _configuration["RabbitMQ:ConnectionString"];

                    cfg.ReceiveEndpoint("vacancy-created-queue", e =>
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<JobDbContext>();
                        var consumer = new VacancyCreatedConsumer(dbContext);

                        e.Consumer(() => consumer);
                    });

                    cfg.ReceiveEndpoint("user-notification-queue", e =>
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<JobDbContext>();
                        var consumer = new UpdateUserApplicationStatusConsumer(dbContext);

                        e.Consumer(() => consumer);
                    });

                    cfg.ReceiveEndpoint("get-resume-data-queue", e =>
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<JobDbContext>();
                        var consumer = new GetResumeDataConsumer(dbContext);

                        e.Consumer(() => consumer);
                    });

                    cfg.ReceiveEndpoint("user-registered-queue", e =>
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<JobDbContext>();
                        var consumer = new UserRegisteredConsumer(dbContext);

                        e.Consumer(() => consumer);
                    });
                });

                await busControl.StartAsync(stoppingToken);

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("RabbitMQ service stopping...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("RabbitMQ service stopped.");
            await base.StopAsync(cancellationToken);
        }
    }
}
