using Job.Business.Consumers;
using Job.Business.Services.Certificate;
using Job.Business.Services.Education;
using Job.Business.Services.Experience;
using Job.Business.Services.Language;
using Job.Business.Services.Notification;
using Job.Business.Services.Number;
using Job.Business.Services.Position;
using Job.Business.Services.Resume;
using Job.Business.Services.Skill;
using Job.Business.Services.User;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.HelperServices.Current;

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
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IPositionService, PositionService>();
        }

        public static IServiceCollection AddMassTransit(this IServiceCollection services, string username, string password, string hostname, string port)
        {
            password = Uri.EscapeDataString(password);
            string cString = $"amqp://{username}:{password}@{hostname}:{port}/";

            //var rabbitMqConfig = configuration.GetSection("RabbitMQ");
            services.AddMassTransit(x =>
            {
                x.AddConsumer<UserRegisteredConsumer>();
                x.AddConsumer<GetResumeDataConsumer>();
                x.AddConsumer<UpdateUserApplicationStatusConsumer>();
                x.AddConsumer<VacancyCreatedConsumer>();
                x.AddConsumer<VacancyUpdatedConsumer>();
                x.AddConsumer<VacancyDeletedConsumer>();
                x.AddConsumer<GetResumeIdsByUserIdsConsumer>();
                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(cString);

                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddMassTransitHostedService();

            return services;
        }
    }
}