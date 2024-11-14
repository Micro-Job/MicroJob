using FluentValidation.AspNetCore;
using JobCompany.Business;
using JobCompany.Business.Dtos.VacancyDtos;
using MassTransit;
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
            builder.Services.AddSwaggerGen();

            builder.Services.AddFluentValidation(opt =>
            {
                opt.RegisterValidatorsFromAssemblyContaining<CreateVacancyDto>();
            });

            builder.Services.AddAuth(builder.Configuration["Jwt:Issuer"]!, builder.Configuration["Jwt:Audience"]!, builder.Configuration["Jwt:SigningKey"]!);
            builder.Services.AddJobCompanyServices();
            builder.Services.AddMassTransitCompany(builder.Configuration["RabbitMQ"]!);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCustomExceptionHandler();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
