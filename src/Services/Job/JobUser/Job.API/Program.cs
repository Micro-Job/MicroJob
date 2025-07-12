using FluentValidation;
using FluentValidation.AspNetCore;
using Job.Business;
using Job.Business.Dtos.ResumeDtos;
using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SharedLibrary.Filters;
using SharedLibrary.Middlewares;
using SharedLibrary.ServiceRegistration;

namespace Job.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            // Add services to the container.
            builder.Services.AddControllers();

            // Add Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwagger();

            builder.Services.AddAuth(
                configuration["Jwt:Issuer"]!,
                configuration["Jwt:Audience"]!,
                configuration["Jwt:SigningKey"]!
            );

            builder.Services.AddDbContext<JobDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("MSSQL"));
            });

            builder.Services.AddValidatorsFromAssemblyContaining<ResumeCreateDto>()
                .AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

            //var IconBuilder = builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
            //                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //                        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

            //var newBuilder = IconBuilder.Build();

            builder.Services.AddMassTransit(builder.Configuration["RabbitMQ:Username"]!, builder.Configuration["RabbitMQ:Password"]!, builder.Configuration["RabbitMQ:Hostname"]!, builder.Configuration["RabbitMQ:Port"]!);

            // Add Job Services
            builder.Services.AddJobServices();

            // CORS policy with multiple allowed origins
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "_myAllowSpecificOrigins",
                    policy =>
                    {
                        policy
                            .WithOrigins(
                                "http://localhost:3000",
                                "http://localhost:3001",
                                "http://localhost:3002",
                                "http://localhost:5000",
                                "https://job.siesco.studio"
                            )
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();

                        policy.WithOrigins("http://localhost:5000").AllowAnyMethod().AllowAnyHeader();
                    }
                );
            });

            var app = builder.Build();

            //app.UseCors("AllowSwagger");
            app.UseCors("_myAllowSpecificOrigins");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMiddleware<LanguageMiddleware>();

            app.UseCustomExceptionHandler();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
