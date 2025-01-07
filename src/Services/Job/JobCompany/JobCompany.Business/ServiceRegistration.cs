using Job.Business.Consumers;
using JobCompany.Business.Consumers;
using JobCompany.Business.Services.AnswerServices;
using JobCompany.Business.Services.ApplicationServices;
using JobCompany.Business.Services.CategoryServices;
using JobCompany.Business.Services.CityServices;
using JobCompany.Business.Services.CompanyServices;
using JobCompany.Business.Services.CountryServices;
using JobCompany.Business.Services.ExamServices;
using JobCompany.Business.Services.NotificationServices;
using JobCompany.Business.Services.QuestionServices;
using JobCompany.Business.Services.ReportServices;
using JobCompany.Business.Services.Skill;
using JobCompany.Business.Services.SkillServices;
using JobCompany.Business.Services.StatusServices;
using JobCompany.Business.Services.VacancyServices;
using MassTransit;
using Microsoft.Extensions.Configuration;
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
            services.AddScoped<IStatusService, StatusService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IAnswerService, AnswerService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IAnswerService, AnswerService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<INotificationService, NotificationService>();
        }

        public static IServiceCollection AddMassTransitCompany(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var rabbitMqConfig = configuration.GetSection("RabbitMQ");
            services.AddMassTransit(x =>
            {
                x.AddConsumer<CompanyRegisteredConsumer>();
                x.AddConsumer<VacancyDataConsumer>();
                x.AddConsumer<GetAllCompaniesConsumer>();
                x.AddConsumer<UserApplicationConsumer>();
                x.AddConsumer<VacancyApplicationConsumer>();
                x.AddConsumer<GetAllVacanciesConsumer>();
                x.AddConsumer<GetCompanyDetailByIdConsumer>();
                x.AddConsumer<SimilarVacanciesConsumer>();
                x.AddConsumer<GetVacancyInfoConsumer>();
                x.AddConsumer<GetAllVacanciesByCompanyIdConsumer>();
                x.AddConsumer<GetUserApplicationsConsumer>();
                x.AddConsumer<CheckVacancyConsumer>();
                x.AddConsumer<CheckCompanyConsumer>();
                x.AddConsumer<GetOtherVacanciesByCompanyConsumer>();
                x.AddConsumer<GetApplicationDetailConsumer>();
                x.AddConsumer<GetExamDetailConsumer>();
                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq(
                    (context, cfg) =>
                    {
                        var rabbitMqConnectionString = configuration["RabbitMQ:ConnectionString"];
                        if (string.IsNullOrEmpty(rabbitMqConnectionString))
                        {
                            throw new InvalidOperationException(
                                "RabbitMQ Connection String is missing."
                            );
                        }

                        cfg.Host(rabbitMqConnectionString);

                        cfg.ConfigureEndpoints(context);
                    }
                );
            });

            return services;
        }
    }
}
