using CreditService.Services;

namespace CreditService.Utils
{
    public class CreditConsumerBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<CreditConsumerBackgroundService> _logger;

        public CreditConsumerBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<CreditConsumerBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Credit Consumer Service started");

            using var scope = _serviceScopeFactory.CreateScope();
            var creditAppService = scope.ServiceProvider.GetRequiredService<ICreditAppService>();

            await creditAppService.StartCreditConsumerAsync(stoppingToken);

            _logger.LogInformation("Credit Consumer Service stopped");
        }
    }


}
