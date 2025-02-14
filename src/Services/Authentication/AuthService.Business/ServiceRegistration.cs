using AuthService.Business.Consumers;
using AuthService.Business.HelperServices.Email;
using AuthService.Business.HelperServices.TokenHandler;
using AuthService.Business.Publishers;
using AuthService.Business.Services.Auth;
using AuthService.Business.Services.CurrentUser;
using AuthService.Business.Services.UserServices;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.ExternalServices.FileService;

namespace AuthService.Business
{
    public static class ServiceRegistration
    {
        public static void AddAuthServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ITokenHandler, TokenHandler>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuthService, Services.Auth.AuthService>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<EmailPublisher>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFileService, FileService>();
        }

        public static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqConfig = configuration.GetSection("RabbitMQ");
            services.AddMassTransit(x =>
            {
                x.AddConsumer<GetUserDataConsumer>();
                x.AddConsumer<GetUserMiniDataConsumer>();
                x.AddConsumer<GetUsersDataConsumer>();
                x.AddConsumer<GetUserEmailConsumer>(); 
                x.AddConsumer<GetUserPhotoConsumer>(); 

                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri($"rabbitmq://{configuration["RabbitMQ:Hostname"]}:{configuration["RabbitMQ:Port"]}"), h =>
                    {
                        h.Username(configuration["RabbitMQ:Username"]);
                        h.Password(configuration["RabbitMQ:Password"]);
                    });

                    //var rabbitMqConnectionString = configuration["RabbitMQ:ConnectionString"];
                    //if (string.IsNullOrEmpty(rabbitMqConnectionString))
                    //{
                    //    throw new InvalidOperationException("RabbitMQ Connection String is missing.");
                    //}

                    //cfg.Host(rabbitMqConnectionString);

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
