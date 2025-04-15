using JobPayment.Business.Consumers;
using JobPayment.Business.Services.BalanceServices;
using JobPayment.Business.Services.DepositServices;
using JobPayment.Business.Services.PacketServices;
using JobPayment.Business.Services.Payment;
using JobPayment.Business.Services.PriceServices;
using JobPayment.Business.Services.TransactionServices;
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
            services.AddScoped<IPriceService , PriceService>();
            services.AddScoped<IPaymentService , PaymentService>();
            services.AddScoped<ITransactionService , TransactionService>();
        }

        public static void AddMassTransit(this IServiceCollection services, string username, string password, string hostname, string port)
        {
            password = Uri.EscapeDataString(password);
            string cString = $"amqp://{username}:{password}@{hostname}:{port}/";

            services.AddMassTransit(x =>
            {
                x.AddConsumer<CreateBalanceConsumer>();
                x.AddConsumer<CheckBalanceConsumer>();
                x.AddConsumer<PayConsumer>();

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
