using AuthService.Business;
using AuthService.Business.Dtos;
using AuthService.DAL.Contexts;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SharedLibrary.Filters;
using SharedLibrary.Middlewares;
using SharedLibrary.ServiceRegistration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
               .AddNewtonsoftJson(options =>
                   options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddValidatorsFromAssemblyContaining<RegisterDto>()
                .AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwagger();

builder.Services.AddAuthServices(builder.Configuration);

builder.Services.AddMassTransit(builder.Configuration["RabbitMQ:Username"]!, builder.Configuration["RabbitMQ:Password"]!, builder.Configuration["RabbitMQ:Hostname"]!, builder.Configuration["RabbitMQ:Port"]!);

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
});

// CORS policy with multiple allowed origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("_myAllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:3002", "http://localhost:5000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();

        policy.WithOrigins("http://localhost:5000").AllowAnyMethod().AllowAnyHeader();
    });
});
builder.Services.AddAuth(builder.Configuration["Jwt:Issuer"]!, builder.Configuration["Jwt:Audience"]!, builder.Configuration["Jwt:SigningKey"]!);

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