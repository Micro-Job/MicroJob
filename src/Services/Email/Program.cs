using EmailService.API.Consumers;
using EmailService.API.Services;
using MassTransit;
using SharedLibrary.Dtos.EmailDtos;
using SharedLibrary.ServiceRegistration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEmailService, EmailService.API.Services.EmailService>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// Configure CORS policy with a specific origin for security
builder.Services.AddCors(options =>
{
    options.AddPolicy("_myAllowSpecificOrigins",
        builder => builder.WithOrigins("http://localhost:3000")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// Configure MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SendEmailConsumer>();
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqHost = builder.Configuration["RabbitMQ:Host"];
        var username = builder.Configuration["RabbitMQ:Username"];
        var password = builder.Configuration["RabbitMQ:Password"];

        var encodedPassword = Uri.EscapeDataString(password);

        cfg.Host(rabbitMqHost, h =>
        {
            h.Username(username);
            h.Password(encodedPassword); 
        });

        cfg.ConfigureEndpoints(context);
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use the configured CORS policy
app.UseCors("_myAllowSpecificOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
