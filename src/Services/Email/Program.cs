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
builder.Services.AddCorsPolicy("http://localhost:3000");

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SendEmailConsumer>();
    x.SetKebabCaseEndpointNameFormatter();
    x.UsingRabbitMq((con, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ"]);
        cfg.ConfigureEndpoints(con);
    });
});

//builder.Services.AddHangfireServer();
//builder.Services.AddHangfire(x =>
//{
//    var option = new SqlServerStorageOptions
//    {
//        PrepareSchemaIfNecessary = true,
//        QueuePollInterval = TimeSpan.FromMinutes(5),
//        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
//        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
//        UseRecommendedIsolationLevel = true,
//        UsePageLocksOnDequeue = true,
//        DisableGlobalLocks = true
//    };

//    x.UseSqlServerStorage(builder.Configuration.GetConnectionString("MSSql"), option)
//    .WithJobExpirationTimeout(TimeSpan.FromMinutes(5));
//});
var app = builder.Build();

//app.UseHangfireServer();
//app.UseHangfireDashboard();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("_myAllowSpecificOrigins");
app.UseAuthorization();

app.MapControllers();

app.Run();
