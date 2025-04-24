using FluentValidation.AspNetCore;
using Job.Business;
//using Job.Business.BackgroundServices;
using Job.Business.Consumers;
using Job.Business.Services.Resume;
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
            builder.Services.AddSwaggerGen(c =>
            {
                c.OperationFilter<AddLanguageHeaderParameter>();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter token",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "bearer",
                    }
                );

                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                            },
                            new string[] { }
                        },
                    }
                );
            });

            builder.Services.AddAuth(
                configuration["Jwt:Issuer"]!,
                configuration["Jwt:Audience"]!,
                configuration["Jwt:SigningKey"]!
            );

            builder.Services.AddDbContext<JobDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("MSSQL"));
            });

            //builder.Services.AddHostedService<RabbitMqBackgroundService>();


            builder.Services.AddFluentValidation(opt =>
            {
                opt.RegisterValidatorsFromAssemblyContaining<ResumeService>();
            });


            var IconBuilder = builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

            var newBuilder = IconBuilder.Build();

            builder.Services.AddMassTransit(newBuilder["RabbitMQ:Username"]!, newBuilder["RabbitMQ:Password"]!, newBuilder["RabbitMQ:Hostname"]!, newBuilder["RabbitMQ:Port"]!);
            //TODO : Bu neye gore var
            //builder.Services.AddHostedService<RabbitMqBackgroundService>();

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
                                "http://localhost:3002"
                            )
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    }
                );
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
            app.UseCors("_myAllowSpecificOrigins");
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
