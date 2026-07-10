namespace FinTrack.API.Infrastructure.Common.DTO
{
    public class TransactionDTO
    {
        public TransactionDTO() { }
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
    }
}
