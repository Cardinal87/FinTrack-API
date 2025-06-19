
namespace FinTrack.API.Infrastructure.Data.DbEntities
{
    public class AccountDb
    {
        public AccountDb() { }
        public Guid Id { get; set; }
        public decimal Balance { get; set; }

        public Guid UserId { get; set; }
        public UserDb User { get; set; } = null!;
        public List<TransactionDb> OutgoingTransactions { get; set; } = new();
        public List<TransactionDb> IncomingTransactions { get; set; } = new();
    }
}
