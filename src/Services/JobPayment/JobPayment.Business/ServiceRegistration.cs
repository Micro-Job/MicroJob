using JobPayment.Business.Consumers;
using JobPayment.Business.Services.BalanceSer;
using JobPayment.Business.Services.DepositSer;
using JobPayment.Business.Services.PacketSer;
using JobPayment.Business.Services.TransactionSer;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.HelperServices.Current;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business
{
    public static class ServiceRegistration
    {
        public static void AddPaymentServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IBalanceService , BalanceService>();
            services.AddScoped<IDepositService , DepositService>();
            services.AddScoped<IPacketService , PacketService>();
            services.AddScoped<ITransactionService , TransactionService>();
        }

        public static void AddMassTransit(this IServiceCollection services, string username, string password, string hostname, string port)
        {
            password = Uri.EscapeDataString(password);
            string cString = $"amqp://{username}:{password}@{hostname}:{port}/";

            services.AddMassTransit(x =>
            {
                x.AddConsumer<CreateBalanceConsumer>();

                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(cString);

                    cfg.ConfigureEndpoints(context);
                });
            });

        }
    }
}
