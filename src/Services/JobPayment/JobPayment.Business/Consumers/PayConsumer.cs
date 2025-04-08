using JobPayment.Business.Dtos.Payment;
using JobPayment.Business.Services.Payment;
using MassTransit;
using SharedLibrary.Events;

namespace JobPayment.Business.Consumers
{
    public class PayConsumer(IPaymentService _paymentService) : IConsumer<PayEvent>
    {
        public async Task Consume(ConsumeContext<PayEvent> context)
        {
            await _paymentService.Pay(new PayDto
            {
                UserId = context.Message.UserId,
                InformationId = context.Message.InformationId,
                InformationType = context.Message.InformationType
            });
        }
    }
}
