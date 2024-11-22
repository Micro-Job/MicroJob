using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using SharedLibrary.ServiceRegistration;
using SharedLibrary.Middlewares;

namespace Job.Gateway
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddOcelot();

            builder.Configuration.AddJsonFile("ocelot.json");

            builder.Services.AddControllers();

            builder.Services.AddCorsPolicy("http://localhost:3000");

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();
            app.UseCors("_myAllowSpecificOrigins");
            app.UseCustomExceptionHandler();
            app.MapControllers();
            await app.UseOcelot();
            app.Run();
        }
    }
}