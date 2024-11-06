using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.ExternalServices;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Job.Business.Services.Resume;
using Job.Core.Entities;
using Job.Business.Services.Education;
using Job.Business.Services.Experience;
using Job.Business.Consumers;

namespace Job.Business
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IResumeService, ResumeService>();
            services.AddScoped<IEducationService, EducationService>();
            services.AddScoped<IExperienceService, ExperienceService>();
        }

        public static IServiceCollection AddMassTransit(this IServiceCollection services, string cString)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<UserRegisteredConsumer>();
                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((con, cfg) =>
                {
                    cfg.Host(cString);
                    cfg.ConfigureEndpoints(con);
                });
            });
            //services.AddMassTransitHostedService();   
            return services;
        }
    }
}