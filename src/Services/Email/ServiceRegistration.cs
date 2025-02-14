using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmailService.API.Consumers;
using EmailService.API.Services;
using MassTransit;

namespace Email
{
    public static class ServiceRegistration
    {
        public static void AddEmailServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IEmailService, EmailService.API.Services.EmailService>();
        }

        public static IServiceCollection AddMassTransitEmail(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqConfig = configuration.GetSection("RabbitMQ");
            services.AddMassTransit(x =>
            {
                x.AddConsumer<SendEmailConsumer>();
                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((context, cfg) =>
                {
                    //var rabbitMqConnectionString = configuration["RabbitMQ:ConnectionString"];
                    //if (string.IsNullOrEmpty(rabbitMqConnectionString))
                    //{
                    //    throw new InvalidOperationException("RabbitMQ Connection String is missing.");
                    //}

                    //cfg.Host(rabbitMqConnectionString);

                    cfg.Host(new Uri($"rabbitmq://{configuration["RabbitMQ:Hostname"]}:{configuration["RabbitMQ:Port"]}"), h =>
                    {
                        h.Username(configuration["RabbitMQ:Username"]);
                        h.Password(configuration["RabbitMQ:Password"]);
                    });


                    cfg.ConfigureEndpoints(context);
                });
            });
            return services;
        }
    }
}