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

namespace JobPayment.Business
{
    public static class ServiceRegistration
    {
        public static void AddPaymentServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<BalanceService>();
            services.AddScoped<DepositService>();
            services.AddScoped<PacketService>();
            services.AddScoped<PriceService>();
            services.AddScoped<PaymentService>();
            services.AddScoped<TransactionService>();
        }

        public static void AddMassTransit(this IServiceCollection services, string username, string password, string hostname, string port)
        {
            password = Uri.EscapeDataString(password);
            string cString = $"amqp://{username}:{password}@{hostname}:{port}/";

            services.AddMassTransit(x =>
            {
                x.AddConsumer<CreateBalanceConsumer>();
                x.AddConsumer<CheckBalanceConsumer>();
                x.AddConsumer<CheckBalancesConsumer>();
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
