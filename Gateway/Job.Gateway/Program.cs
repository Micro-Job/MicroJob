using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using SharedLibrary.Middlewares;
using SharedLibrary.ServiceRegistration;

namespace Job.Gateway
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var environment = builder.Environment.EnvironmentName;

            if (environment == "Development")
            {
                builder.Configuration
                                 .AddJsonFile($"ocelot.Development.json", optional: true, reloadOnChange: true);
            }
            else if (environment == "Test")
            {
                builder.Configuration
                                 .AddJsonFile($"ocelot.Test.json", optional: true, reloadOnChange: true);
            }
            else
            {
                builder.Configuration.AddJsonFile("ocelot.json", optional: true, reloadOnChange: true);
            }

            builder.Services.AddOcelot();
            builder.Services.AddSwaggerForOcelot(builder.Configuration);

            builder.Services.AddControllers();

            // CORS policy with multiple allowed origins
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("_myAllowSpecificOrigins", policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:3002", "https://job.siesco.studio")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();

                    policy.WithOrigins("http://localhost:5000",
                            "http://localhost:5001",
                            "http://localhost:5002",
                            "http://localhost:5082",
                            "http://localhost:5003").AllowAnyMethod().AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseCors("AllowSwagger");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("http://localhost:5001/swagger/v1/swagger.json", "Auth Service");
                c.SwaggerEndpoint("http://localhost:5002/swagger/v1/swagger.json", "User Service");
                c.SwaggerEndpoint("http://localhost:5082/swagger/v1/swagger.json", "Company Service");
                c.SwaggerEndpoint("http://localhost:5003/swagger/v1/swagger.json", "Payment Service");
                c.RoutePrefix = "swagger";
                c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
            });

            app.UseAuthorization();
            app.UseCustomExceptionHandler();
            app.UseCors("_myAllowSpecificOrigins");
            app.MapControllers();
            await app.UseOcelot();
            app.Run();
        }
    }
}
