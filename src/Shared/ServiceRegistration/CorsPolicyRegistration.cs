using Microsoft.Extensions.DependencyInjection;

namespace SharedLibrary.ServiceRegistration
{
    public static class CorsPolicyRegistration
    {
        public static void AddCorsPolicy(this IServiceCollection services, params string[] origins)
        {
            services.AddCors(opt =>
            {
                opt.AddPolicy(name: "_myAllowSpecificOrigins",
                    builder =>
                    {
                        if (origins != null && origins.Length > 0)
                        {
                            builder.WithOrigins(origins)
                                   .AllowAnyHeader()
                                   .AllowAnyMethod()
                                   .AllowCredentials();
                        }
                        else
                        {
                            builder.AllowAnyOrigin()
                                   .AllowAnyHeader()
                                   .AllowAnyMethod()
                                   .AllowCredentials();
                        }
                    });
            });
        }
    }
}
//builder.Services.AddCorsPolicy("http://localhost:5173", "https://localhost:7069");
