﻿using JobCompany.Business.Consumers;
using JobCompany.Business.Services.VacancyServices;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.ExternalServices.FileService;

namespace JobCompany.Business
{
    public static class ServiceRegistration
    {
        public static void AddJobCompanyServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IVacancyService, VacancyService>();
        }

        public static IServiceCollection AddMassTransitCompany(this IServiceCollection services, string cString)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<CompanyRegisteredConsumer>();
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
