using MassTransit;
using Microsoft.Extensions.Hosting;
using SharedLibrary.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.BackgroundServices
{
    public class PeriodicPayPublisherService(IPublishEndpoint _publishEndpoint) : BackgroundService
    {
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _publishEndpoint.Publish(new PeriodicVacancyPayEvent());

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
