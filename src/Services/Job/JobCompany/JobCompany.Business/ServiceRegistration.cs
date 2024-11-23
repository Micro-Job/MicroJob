using AuthService.Business.Services.CurrentUser;
using JobCompany.Business.Consumers;
using JobCompany.Business.Services.ApplicationServices;
using JobCompany.Business.Services.CategoryServices;
using JobCompany.Business.Services.CountryServices;
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
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IStatusService, StatusService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICountryService, CountryService>();
        }

        public static IServiceCollection AddMassTransitCompany(this IServiceCollection services, string cString)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<CompanyRegisteredConsumer>();
                x.AddConsumer<VacancyDataConsumer>();
                x.AddConsumer<GetAllCompaniesConsumer>();
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