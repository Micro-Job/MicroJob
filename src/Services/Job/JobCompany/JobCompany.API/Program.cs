using FluentValidation.AspNetCore;
using JobCompany.Business;
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SharedLibrary.Filters;
using SharedLibrary.Middlewares;
using SharedLibrary.ServiceRegistration;

namespace JobCompany.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.OperationFilter<AddLanguageHeaderParameter>();
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
                opt.AddSecurityDefinition(
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

                opt.AddSecurityRequirement(
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
            builder.Services.AddDbContext<JobCompanyDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"))
            );

            builder.Services.AddFluentValidation(opt =>
            {
                opt.RegisterValidatorsFromAssemblyContaining<CreateVacancyDto>();
            });

            builder.Services.AddAuth(
                builder.Configuration["Jwt:Issuer"]!,
                builder.Configuration["Jwt:Audience"]!,
                builder.Configuration["Jwt:SigningKey"]!
            );
            builder.Services.AddJobCompanyServices();
            var IconBuilder = builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                                       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

            var newBuilder = IconBuilder.Build();

            builder.Services.AddMassTransit(newBuilder["RabbitMQ:Username"]!, newBuilder["RabbitMQ:Password"]!, newBuilder["RabbitMQ:Hostname"]!, newBuilder["RabbitMQ:Port"]!);

            //builder.Services.AddMassTransit(builder.Configuration);

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
            app.UseStaticFiles();
            app.UseMiddleware<LanguageMiddleware>();

            // app.UseCustomExceptionHandler();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("_myAllowSpecificOrigins");
            app.MapControllers();

            app.Run();
        }
    }
}
