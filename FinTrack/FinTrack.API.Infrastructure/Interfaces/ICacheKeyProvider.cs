namespace FinTrack.API.Infrastructure.Interfaces
{
    public interface ICacheKeyProvider
    {
        string UserById(Guid id);
        string UserByEmail(string email);
        string UserAll(int page, int size);
        string AccountById(Guid id);
        string AccountsByUserId(Guid userId);
        string AccountAll(int page, int size);
        string TransactionsById(Guid id);
        string UserListPattern();
        string AccountListPattern();
        string RefreshToken(string hash);

    }
}
