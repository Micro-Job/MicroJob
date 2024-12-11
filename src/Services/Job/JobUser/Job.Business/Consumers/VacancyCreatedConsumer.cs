using Job.Business.Dtos.NotificationDtos;
using Job.Business.Services.Notification;
using Job.Core.Entities;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedLibrary.Events;
using System.Text;
using System.Text.Json;

namespace Job.Business.Consumers
{
    public class VacancyCreatedConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public VacancyCreatedConsumer(IServiceProvider serviceProvider, IConnectionFactory connectionFactory)
        {
            _serviceProvider = serviceProvider;

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "vacancy-created-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var vacancyCreatedEvent = JsonSerializer.Deserialize<VacancyCreatedEvent>(message);

                if (vacancyCreatedEvent != null)
                {
                    await ProcessVacancyCreatedEventAsync(vacancyCreatedEvent);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: "vacancy-created-queue", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        private async Task ProcessVacancyCreatedEventAsync(VacancyCreatedEvent vacancyCreatedEvent)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<JobDbContext>();

            var matchingUsers = await context.Resumes
                .Where(r => r.ResumeSkills.Any(rs => vacancyCreatedEvent.SkillIds.Contains(rs.SkillId)))
                .ToListAsync();

            foreach (var resume in matchingUsers)
            {
                await SendNotificationAsync(resume, vacancyCreatedEvent);
            }
        }

        private async Task SendNotificationAsync(Resume resume, VacancyCreatedEvent vacancyEvent)
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var notificationDto = new NotificationDto
            {
                ReceiverId = resume.UserId,
                SenderId = vacancyEvent.CreatedById,
                Content = $"{vacancyEvent.Title} - vakansiyası sizin bacarıqlarınıza uyğundur."
            };

            await notificationService.CreateNotificationAsync(notificationDto);
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}