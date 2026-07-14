    using FinTrack.API.Infrastructure.Interfaces;

namespace FinTrack.API.Infrastructure.Caching.Services
{
    public class RedisKeyProvider : ICacheKeyProvider
    {
        public string AccountAll(int page, int size) => $"account:all:{page}:{size}";

        public string AccountById(Guid id) => $"account:{id}";

        public string AccountsByUserId(Guid userId) => $"account:user:{userId}";

        public string AccountListPattern() => "account:all:*";

        public string UserListPattern() => "user:all:*";

        public string TransactionsById(Guid id) => $"transaction:{id}";

        public string UserAll(int page, int size) => $"user:all:{page}:{size}";

        public string UserByEmail(string email) => $"user:email:{email}";

        public string UserById(Guid id) => $"user:{id}";
    }
}

