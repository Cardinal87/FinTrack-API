

namespace FinTrack.API.Infrastructure.Data.DbEntities
{
    public class UserDb
    {
        public UserDb() { }
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public List<AccountDb> Accounts { get; set; } = new();
        
    }
}
