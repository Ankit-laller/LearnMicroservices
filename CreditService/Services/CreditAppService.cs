using CreditService.Dtos;
using CreditService.Utils;
using System.Text.Json;

namespace CreditService.Services
{
    public class CreditAppService : ICreditAppService
    {
        public Task<CreditResponseDto> AddCreditLimitAsync(AddOrUpdateCreditLimitDto input)
        {
            throw new NotImplementedException();
        }

        public Task<CreditResponseDto> CheckCreditAsync(int userId, decimal amount)
        {
            var creditLimit = 3000;

            if (creditLimit < amount)
            {

                return Task.FromResult(
                    new CreditResponseDto
                    {
                        Success = false,
                        AvailibleBalance = creditLimit,
                        Message = "Insufficient balance"
                    });
            }

            return Task.FromResult(new CreditResponseDto
            {
                Success = true,
                AvailibleBalance = creditLimit - amount,
                Message = "Debit successful"
            });
        }

        public Task<CreditResponseDto> GetCreditLimitAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task StartCreditConsumerAsync(CancellationToken token)
        {
            //await RabbitMqHelper.ReceiveMessagesAsync(async (msg) =>
            //{
            //    var request = JsonSerializer.Deserialize<dynamic>(msg);

            //    if (request != null)
            //    {
            //        var response = await CheckCreditAsync(request.AgentId, request.Amount);
            //       // _logger.LogInformation("Processed booking {BookingId}: {Message}", request.BookingId, response.Message);
            //    }

            //}, "booking-to-credit-queue", token);
            // Startup / Program.cs of CreditService
            await RabbitMqHelper.StartConsumerAsync<CreditRequestDto, CreditResponseDto>(
                queueName: "credit-requests",
                handler: async (request) =>
                {
                    // Here we call your business logic
                    return await CheckCreditAsync(request.UserId, request.Amount);
                },
                cancellationToken: token);

        }

        public Task<CreditResponseDto> UpdateCreditLimitAsync(AddOrUpdateCreditLimitDto input)
        {
            throw new NotImplementedException();
        }
    }
}
