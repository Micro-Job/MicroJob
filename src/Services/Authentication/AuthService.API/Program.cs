using AuthService.Business;
using AuthService.Business.Dtos;
using AuthService.DAL.Contexts;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SharedLibrary.Filters;
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
    opt.OperationFilter<AddLanguageHeaderParameter>();
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


builder.Services.AddAuthServices(builder.Configuration);

var IconBuilder = builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

var newBuilder = IconBuilder.Build();

builder.Services.AddMassTransit(newBuilder["RabbitMQ:Username"]!, newBuilder["RabbitMQ:Password"]!, newBuilder["RabbitMQ:Hostname"]!, newBuilder["RabbitMQ:Port"]!);

//builder.Services.AddMassTransit(builder.Configuration);

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
});

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
builder.Services.AddAuth(builder.Configuration["Jwt:Issuer"]!, builder.Configuration["Jwt:Audience"]!, builder.Configuration["Jwt:SigningKey"]!);

var app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<LanguageMiddleware>();

// app.UseCustomExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("_myAllowSpecificOrigins");
app.MapControllers();

app.Run();