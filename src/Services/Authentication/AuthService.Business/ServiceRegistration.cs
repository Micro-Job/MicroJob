using AuthService.Business.Consumers;
using AuthService.Business.HelperServices.Email;
using AuthService.Business.HelperServices.TokenHandler;
using AuthService.Business.Publishers;
using AuthService.Business.Services.Auth;
using AuthService.Business.Services.UserServices;
using AuthService.Business.Templates;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Dtos.EmailDtos;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.HelperServices.Current;

namespace AuthService.Business
{
    public static class ServiceRegistration
    {
        public static void AddAuthServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));

            services.AddHttpContextAccessor();
            services.AddScoped<ITokenHandler, TokenHandler>();
            services.AddScoped<EmailService>();
            services.AddScoped<AuthenticationService>();
            services.AddScoped<EmailPublisher>();
            services.AddScoped<UserService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddSingleton<EmailTemplate>();
        }

        public static IServiceCollection AddMassTransit(this IServiceCollection services, string username, string password, string hostname, string port)
        {
            password = Uri.EscapeDataString(password);
            string cString = $"amqp://{username}:{password}@{hostname}:{port}/";

            services.AddMassTransit(x =>
            {
                x.AddConsumer<GetUserDataConsumer>();
                x.AddConsumer<GetUserMiniDataConsumer>();
                x.AddConsumer<GetUsersDataConsumer>();
                x.AddConsumer<UpdateUserJobStatusEventConsumer>();              

                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((context, cfg) =>
                {
                    //if (string.IsNullOrEmpty(cString))
                    //{
                    //    h.Username(configuration["RabbitMQ:Username"]);
                    //    h.Password(configuration["RabbitMQ:Password"]);
                    //});

                    //var rabbitMqConnectionString = configuration["RabbitMQ:ConnectionString"];
                    //if (string.IsNullOrEmpty(rabbitMqConnectionString))
                    //{
                    //    throw new InvalidOperationException("RabbitMQ Connection String is missing.");
                    //}
                    cfg.Host(cString);
                    //cfg.Host(rabbitMqConnectionString);

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
