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
            builder.Services.AddOcelot();
            builder.Services.AddSwaggerForOcelot(builder.Configuration);
            //builder.Configuration.AddJsonFile("ocelot.json");

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
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseCors("AllowSwagger");
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("http://localhost:5001/swagger/index.html", "Auth Service");
                    c.SwaggerEndpoint("http://localhost:5002/swagger/index.html", "User Service");
                    c.SwaggerEndpoint("http://localhost:5082/swagger/index.html", "Company Service");
                    c.SwaggerEndpoint("http://localhost:5003/swagger/index.html", "Payment Service");
                    c.RoutePrefix = "swagger";
                    c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
                });
            }
            
            app.UseAuthorization();
            app.UseCustomExceptionHandler();
            app.UseCors("_myAllowSpecificOrigins");
            app.MapControllers();
            await app.UseOcelot();
            app.Run();
        }
    }
}
