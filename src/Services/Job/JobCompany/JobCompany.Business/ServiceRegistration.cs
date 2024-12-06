using JobCompany.Business.Consumers;
using JobCompany.Business.Services.ApplicationServices;
using JobCompany.Business.Services.CategoryServices;
using JobCompany.Business.Services.CityServices;
using JobCompany.Business.Services.CompanyServices;
using JobCompany.Business.Services.CountryServices;
using JobCompany.Business.Services.ExamServices.QuestionServices;
using JobCompany.Business.Services.ReportServices;
using JobCompany.Business.Services.StatusServices;
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
            services.AddScoped<IStatusService, StatusService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IQuestionService, QuestionService>();
        }

        public static IServiceCollection AddMassTransitCompany(this IServiceCollection services, string cString)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<CompanyRegisteredConsumer>();
                x.AddConsumer<VacancyDataConsumer>();
                x.AddConsumer<GetAllCompaniesConsumer>();
                x.AddConsumer<UserApplicationConsumer>();
                x.AddConsumer<VacancyApplicationConsumer>();
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