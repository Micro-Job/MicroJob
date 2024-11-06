﻿using AuthService.Business.Consumers;
using AuthService.Business.HelperServices.Email;
using AuthService.Business.HelperServices.TokenHandler;
using AuthService.Business.Publishers;
using AuthService.Business.Services.Auth;
using AuthService.Business.Services.CurrentUser;
using AuthService.Business.Services.UserServices;
using Job.Business.ExternalServices;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Business
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenHandler, TokenHandler>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuthService, Services.Auth.AuthService>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<EmailPublisher>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFileService, FileService>();
        }
        public static IServiceCollection AddMassTransit(this IServiceCollection services, string cString)
        {
            services.AddMassTransit(x=>
            {
                x.AddConsumer<GetUserDataConsumer>();

                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((con, cfg) =>
                {
                    cfg.Host(cString);
                    cfg.ConfigureEndpoints(con);
                });
            });
            return services;
        }

    }
}
