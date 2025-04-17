using Microsoft.Extensions.Options;
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
            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
                                 .AddJsonFile($"ocelot.{environment}.json", optional: true, reloadOnChange: true);

            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("_myAllowSpecificOrigins", policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:3002", "https://job.siesco.studio")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            builder.Services.AddOcelot();
            builder.Services.AddSwaggerForOcelot(builder.Configuration);
            builder.Services.AddControllers();

            var app = builder.Build();

            app.UseAuthorization();
            app.UseCustomExceptionHandler();
            app.UseCors("_myAllowSpecificOrigins");
            app.MapControllers();

           app.UseSwaggerForOcelotUI(opt =>
           {
                opt.PathToSwaggerGenerator = "/swagger/docs";
           }).UseOcelot().Wait();

            //await app.UseOcelot();

            app.Run();

        }
    }
}
