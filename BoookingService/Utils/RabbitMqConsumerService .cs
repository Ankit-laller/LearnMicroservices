namespace BoookingService.Utils
{
    public class RabbitMqConsumerService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           // await RabbitMqHelper.ReceiveMessagesAsync(stoppingToken);
        }
    }
}
