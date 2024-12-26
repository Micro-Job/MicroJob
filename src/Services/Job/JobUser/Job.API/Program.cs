using FluentValidation.AspNetCore;
using Job.Business;
using Job.Business.BackgroundServices;
using Job.Business.Consumers;
using Job.Business.Services.Resume;
using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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

            builder.Services.AddMassTransit(x =>
            {
                // Add consumers for each queue
                x.AddConsumer<VacancyCreatedConsumer>();
                x.AddConsumer<UpdateUserApplicationStatusConsumer>();
                x.AddConsumer<GetResumeDataConsumer>();
                x.AddConsumer<UserRegisteredConsumer>();  

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:ConnectionString"]);

                    cfg.ReceiveEndpoint("vacancy-created-queue", e =>
                    {
                        e.ConfigureConsumer<VacancyCreatedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("user-notification-queue", e =>
                    {
                        e.ConfigureConsumer<UpdateUserApplicationStatusConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("get-resume-data-queue", e =>
                    {
                        e.ConfigureConsumer<GetResumeDataConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("user-registered-queue", e =>
                    {
                        e.ConfigureConsumer<UserRegisteredConsumer>(context); 
                    });
                });
            });

            builder.Services.AddHostedService<RabbitMqBackgroundService>();

            // Add Job Services
            builder.Services.AddJobServices();

            // CORS policy with multiple allowed origins
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("_myAllowSpecificOrigins", policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:3002")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

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
