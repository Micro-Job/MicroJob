using AuthService.Business;
using AuthService.Business.Dtos;
using AuthService.DAL.Contexts;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SharedLibrary.Middlewares;
using SharedLibrary.ServiceRegistration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
               .AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<RegisterDto>())
               .AddNewtonsoftJson(options =>
                   options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
        });
});


builder.Services.AddAuthServices();
builder.Services.AddMassTransit(builder.Configuration["RabbitMQ"]!);

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
});

builder.Services.AddCorsPolicy();
builder.Services.AddAuth(builder.Configuration["Jwt:Issuer"]!, builder.Configuration["Jwt:Audience"]!, builder.Configuration["Jwt:SigningKey"]!);

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

app.UseAuthentication();
app.UseAuthorization();

//app.UseCustomExceptionHandler();

app.MapControllers();

app.Run();
