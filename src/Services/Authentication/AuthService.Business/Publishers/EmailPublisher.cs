using MassTransit;
using SharedLibrary.Dtos.EmailDtos;

namespace AuthService.Business.Publishers
{
    public class EmailPublisher(IBus _bus)
    {
        public async Task SendEmail(EmailMessage dto)
        {
            await _bus.GetSendEndpoint(new Uri("queue:send-email"));
            await _bus.Publish(dto);
        }
    }
}
