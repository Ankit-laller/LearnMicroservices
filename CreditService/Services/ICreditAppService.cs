using CreditService.Dtos;

namespace CreditService.Services
{
    public interface ICreditAppService
    {
        Task<CreditResponseDto> CheckCreditAsync(int userId, decimal amount);
        Task<CreditResponseDto> GetCreditLimitAsync(int userId);
        Task<CreditResponseDto> UpdateCreditLimitAsync(AddOrUpdateCreditLimitDto input);
        Task<CreditResponseDto> AddCreditLimitAsync(AddOrUpdateCreditLimitDto input);
        Task StartCreditConsumerAsync(CancellationToken stoppingToken);
    }
}
