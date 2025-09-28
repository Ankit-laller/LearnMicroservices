namespace CreditService.Dtos
{
    public class AddOrUpdateCreditLimitDto
    {
        public decimal NewLimit { get; set; }
        public int UserId { get; set; }
    }
}
