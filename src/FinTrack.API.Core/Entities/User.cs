using FinTrack.API.Core.Common;

namespace FinTrack.API.Core.Entities
{
    
    /// <summary>
    /// presents User entity
    /// </summary>
    public class User : Entity
    {
        public User(string email, string phone, string name, string hash, Guid accountId)
        {
            Email = email;
            Phone = phone;
            Name = name;
            PasswordHash = hash;
            AccountId = accountId;
        }

        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string Name { get; private set; }
        public string PasswordHash { get; private set; }

        public Guid AccountId { get; private set; }

    }
}
