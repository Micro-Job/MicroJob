using FluentValidation.AspNetCore;
using Job.Business;
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Job.API", Version = "v1" });
            });
             
            builder.Services.AddDbContext<JobDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
            });

            builder.Services.AddFluentValidation(opt =>
            {
                opt.RegisterValidatorsFromAssemblyContaining<ResumeService>();
            });

            builder.Services.AddMassTransit(builder.Configuration["RabbitMQ"]!);

            builder.Services.AddJobServices();
            builder.Services.AddCorsPolicy();

            var app = builder.Build();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Job.API v1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            //app.UseCustomExceptionHandler();

            app.MapControllers();

            app.Run();
        }
    }
}