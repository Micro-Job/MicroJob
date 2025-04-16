using JobCompany.Business.Consumers;
using JobCompany.Business.Services.AnswerServices;
using JobCompany.Business.Services.ApplicationServices;
using JobCompany.Business.Services.CategoryServices;
using JobCompany.Business.Services.CityServices;
using JobCompany.Business.Services.CompanyServices;
using JobCompany.Business.Services.CountryServices;
using JobCompany.Business.Services.ExamServices;
using JobCompany.Business.Services.ManageService;
using JobCompany.Business.Services.NotificationServices;
using JobCompany.Business.Services.QuestionServices;
using JobCompany.Business.Services.ReportServices;
using JobCompany.Business.Services.Skill;
using JobCompany.Business.Services.SkillServices;
using JobCompany.Business.Services.StatusServices;
using JobCompany.Business.Services.VacancyComment;
using JobCompany.Business.Services.VacancyServices;
using MassTransit;
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
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IVacancyCommentService, VacancyCommentService>();
            services.AddScoped<IManageService, ManageService>();

        }

        public static IServiceCollection AddMassTransit(this IServiceCollection services, string username, string password, string hostname, string port)
        {
            password = Uri.EscapeDataString(password);
            string cString = $"amqp://{username}:{password}@{hostname}:{port}/";

            services.AddMassTransit(x =>
            {
                x.AddConsumer<CompanyRegisteredConsumer>();
                x.AddConsumer<UserApplicationConsumer>();
                x.AddConsumer<VacancyApplicationConsumer>();
                x.AddConsumer<GetAllVacanciesConsumer>();
                x.AddConsumer<GetCompanyDetailByIdConsumer>();
                x.AddConsumer<GetAllVacanciesByCompanyIdConsumer>();
                x.AddConsumer<CheckVacancyConsumer>();
                x.AddConsumer<VacancyRejectConsumer>();
                x.AddConsumer<GetCompaniesDataByUserIdsConsumer>();
                x.AddConsumer<VacancyAcceptConsumer>();

                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq(
                    (context, cfg) =>
                    {
                        cfg.Host(cString);

                        cfg.ConfigureEndpoints(context);
                    }
                );
            });

            return services;
        }
    }
}