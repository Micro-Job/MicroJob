using JobCompany.Business;
using MassTransit;
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

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}