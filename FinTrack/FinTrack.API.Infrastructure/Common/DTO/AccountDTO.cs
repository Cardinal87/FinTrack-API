namespace FinTrack.API.Infrastructure.Common.DTO
{
    public class AccountDTO
    {
        public AccountDTO() { }
        public Guid Id { get; set; }
        public decimal Balance { get; set; }

        public Guid UserId { get; set; }
    }
}
