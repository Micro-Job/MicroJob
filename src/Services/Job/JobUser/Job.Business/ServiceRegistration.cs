using Job.Business.Consumers;
using Job.Business.Services;
using Job.Business.Services.Certificate;
using Job.Business.Services.Education;
using Job.Business.Services.Experience;
using Job.Business.Services.Language;
using Job.Business.Services.Notification;
using Job.Business.Services.Number;
using Job.Business.Services.Position;
using Job.Business.Services.Resume;
using Job.Business.Services.Skill;
using Job.Business.Services.UserManagement;
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
            services.AddScoped<ResumeService>();
            services.AddScoped<EducationService>();
            services.AddScoped<UserInformationService>();
            services.AddScoped<ExperienceService>();
            services.AddScoped<NumberService>();
            services.AddScoped<LanguageService>();
            services.AddScoped<CertificateService>();
            services.AddScoped<SkillService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<PositionService>();
            services.AddScoped<UserManagementService>();
        }

        public static IServiceCollection AddMassTransit(this IServiceCollection services, string username, string password, string hostname, string port)
        {
            password = Uri.EscapeDataString(password);
            string cString = $"amqp://{username}:{password}@{hostname}:{port}/";

            services.AddMassTransit(x =>
            {
                x.AddConsumer<UserRegisteredConsumer>();
                x.AddConsumer<GetResumeDataConsumer>();
                x.AddConsumer<VacancyCreatedConsumer>();
                //x.AddConsumer<GetFilteredUserIdsConsumer>();
                x.AddConsumer<NotificationEventConsumer>();

                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(cString);
                    cfg.ConfigureEndpoints(context);
                });
            });

            //services.AddMassTransitHostedService();

            return services;
        }
    }
}