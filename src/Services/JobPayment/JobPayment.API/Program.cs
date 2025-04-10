using JobPayment.Business;
using JobPayment.DAL.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Middlewares;
using SharedLibrary.ServiceRegistration;

namespace JobPayment.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            builder.Services.AddControllers();

            builder.Services.AddSwagger();

            builder.Services.AddAuth(
               configuration["Jwt:Issuer"]!,
               configuration["Jwt:Audience"]!,
               configuration["Jwt:SigningKey"]!
            );

            //var IconBuilder = builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
            //                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //                        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

            //var newBuilder = IconBuilder.Build();

            builder.Services.AddPaymentServices();

            builder.Services.AddMassTransit(configuration["RabbitMQ:Username"]!, configuration["RabbitMQ:Password"]!, configuration["RabbitMQ:Hostname"]!, configuration["RabbitMQ:Port"]!);

            builder.Services.AddDbContext<PaymentDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("MSSQL"));
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
                });
            }
            app.UseHttpsRedirection();

            //app.UseCustomExceptionHandler();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
