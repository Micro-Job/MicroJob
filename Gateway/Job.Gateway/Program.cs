using Ocelot.DependencyInjection;
using Ocelot.Middleware;

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

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            app.MapControllers();
            await app.UseOcelot();
            app.Run();
        }
    }
}
