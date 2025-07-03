using JobCompany.Business.BackgroundServices;
using JobCompany.Business.Consumers;
using JobCompany.Business.Services.AnswerServices;
using JobCompany.Business.Services.ApplicationServices;
using JobCompany.Business.Services.CategoryServices;
using JobCompany.Business.Services.CityServices;
using JobCompany.Business.Services.CompanyServices;
using JobCompany.Business.Services.CountryServices;
using JobCompany.Business.Services.DashboardService;
using JobCompany.Business.Services.ExamServices;
using JobCompany.Business.Services.ManageService;
using JobCompany.Business.Services.NotificationServices;
using JobCompany.Business.Services.QuestionServices;
using JobCompany.Business.Services.ReportServices;
using JobCompany.Business.Services.SkillServices;
using JobCompany.Business.Services.StatusServices;
using JobCompany.Business.Services.VacancyComment;
using JobCompany.Business.Services.VacancyServices;
using MassTransit;
using MassTransit.Middleware;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business
{
    public static class ServiceRegistration
    {
        public static void AddJobCompanyServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddHostedService<PeriodicPayPublisherService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<VacancyService>();
            services.AddScoped<StatusService>();
            services.AddScoped<ApplicationService>();
            services.AddScoped<CategoryService>();
            services.AddScoped<CountryService>();
            services.AddScoped<CompanyService>();
            services.AddScoped<CityService>();
            services.AddScoped<ReportService>();
            services.AddScoped<QuestionService>();
            services.AddScoped<AnswerService>();
            services.AddScoped<ExamService>();
            services.AddScoped<AnswerService>();
            services.AddScoped<SkillService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<VacancyCommentService>();
            services.AddScoped<ManageService>();
            services.AddScoped<DashboardService>();

        }

        public static IServiceCollection AddMassTransit(this IServiceCollection services, string username, string password, string hostname, string port)
        {
            password = Uri.EscapeDataString(password);
            string cString = $"amqp://{username}:{password}@{hostname}:{port}/";

            services.AddMassTransit(x =>
            {
                x.AddConsumer<CompanyRegisteredConsumer>();                                           
                x.AddConsumer<GetCompaniesDataByUserIdsConsumer>();
                x.AddConsumer<VacancyAcceptConsumer>();
                x.AddConsumer<CheckApplicationConsumer>();
                x.AddConsumer<PeriodicVacancyPayConsumer>();
                x.AddConsumer<CheckVOENConsumer>();

                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(cString);

                    cfg.ConfigureEndpoints(context);
                });

                x.AddInMemoryInboxOutbox();
            });

            return services;
        }
    }
}