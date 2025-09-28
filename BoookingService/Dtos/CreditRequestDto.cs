namespace BoookingService.Dtos
{
    public class CreditRequestDto
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string? CorrelationId { get; set; }
    }

    public class CreditResponseDto
    {
        public bool Success { get; set; }
        public decimal AvailibleBalance { get; set; }
        public string Message { get; set; } = "";
    }

}
