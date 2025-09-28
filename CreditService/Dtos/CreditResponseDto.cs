namespace CreditService.Dtos
{
    public class CreditResponseDto
    {
        public bool Success { get; set; }
        public decimal? AvailibleBalance { get; set; } = null;
        public string Message { get; set; } = string.Empty;
    }

    public class CreditRequestDto
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string? CorrelationId { get; set; }
    }


}
