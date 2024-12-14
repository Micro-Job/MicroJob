using Job.Business.Consumers;
using Job.Business.Services.Application;
using Job.Business.Services.Certificate;
using Job.Business.Services.Company;
using Job.Business.Services.Education;
using Job.Business.Services.Experience;
using Job.Business.Services.Language;
using Job.Business.Services.Notification;
using Job.Business.Services.Number;
using Job.Business.Services.Resume;
using Job.Business.Services.Skill;
using Job.Business.Services.User;
using Job.Business.Services.Vacancy;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.ExternalServices.FileService;

namespace Job.Business
{
    public static class ServiceRegistration
    {
        public static void AddJobServices(this IServiceCollection services)
        {
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IResumeService, ResumeService>();
            services.AddScoped<IEducationService, EducationService>();
            services.AddScoped<IUserInformationService, UserInformationService>();
            services.AddScoped<IExperienceService, ExperienceService>();
            services.AddScoped<INumberService, NumberService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<ICertificateService, CertificateService>();
            services.AddScoped<IVacancyService, VacancyService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserApplicationService, UserApplicationService>();
            services.AddScoped<ICompanyInformationService, CompanyInformationService>();
        }

        public static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqConfig = configuration.GetSection("RabbitMQ");
            services.AddMassTransit(x =>
            {
                x.AddConsumer<UserRegisteredConsumer>();
                x.AddConsumer<GetResumeDataConsumer>();
                x.AddConsumer<UpdateUserApplicationStatusConsumer>();
                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:ConnectionString"]);

                    cfg.ConfigureEndpoints(context);
                });
            });
            //services.AddMassTransitHostedService();
            return services;
        }
    }
}