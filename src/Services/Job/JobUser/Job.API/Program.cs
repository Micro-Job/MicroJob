using FluentValidation.AspNetCore;
using Job.Business;
using Job.Business.Consumers;
using Job.Business.Services.Resume;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            builder.Services.AddAuth(configuration["Jwt:Issuer"]!, configuration["Jwt:Audience"]!, configuration["Jwt:SigningKey"]!);

            builder.Services.AddDbContext<JobDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("MSSQL"));
            });

            builder.Services.AddFluentValidation(opt =>
            {
                opt.RegisterValidatorsFromAssemblyContaining<ResumeService>();
            });

            // RabbitMQ Connection Factory
            //builder.Services.AddSingleton<IConnectionFactory>(sp =>
            //{
            //    return new ConnectionFactory
            //    {
            //        HostName = configuration["RabbitMQ:HostName"],
            //        UserName = configuration["RabbitMQ:UserName"],
            //        Password = configuration["RabbitMQ:Password"]
            //    };
            //});

            // Add Background Service
            builder.Services.AddHostedService<VacancyCreatedConsumer>();

            builder.Services.AddMassTransit(configuration["RabbitMQ"]!);
            builder.Services.AddJobServices();
            builder.Services.AddCorsPolicy("http://localhost:3000");

            var app = builder.Build();
            app.UseStaticFiles();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseCustomExceptionHandler();
            app.UseCors("_myAllowSpecificOrigins");

            app.MapControllers();

            app.Run();
        }
    }
}