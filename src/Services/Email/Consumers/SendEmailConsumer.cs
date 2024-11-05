using EmailService.API.Services;
using MassTransit;
using SharedLibrary.Dtos.EmailDtos;

namespace EmailService.API.Consumers
{
    public class SendEmailConsumer(IEmailService _service) : IConsumer<EmailMessage>
    {

        public async Task Consume(ConsumeContext<EmailMessage> context)
        {
            await _service.SendMessageAsync(context.Message);
        }
    }
}
