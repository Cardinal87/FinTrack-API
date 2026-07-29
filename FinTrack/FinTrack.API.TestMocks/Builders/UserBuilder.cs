

using FinTrack.API.Core.Entities;
using FinTrack.API.Application.Interfaces;
using FinTrack.API.Infrastructure.Common.DTO;
using System.Runtime.InteropServices;

namespace FinTrack.API.TestMocks.Builders
{
    public class UserBuilder
    {

        private string _name;
        private string _email;
        private string _phone;
        private string _hash = "SHA256.50.Y0ea1poJCyWCd+yPum+ZQZov+ySJgVEGV8lEzNEUjpc=.XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=";
        private List<string> _roles = [];
        private string _nonce = Guid.NewGuid().ToString()[..8];
        private string? _totpSecret = null;
        private bool isEmailVerified = false;
        

        public UserBuilder()
        {
            _name = $"test_user_{_nonce}";
            _email = $"test_{_nonce}@email.com";
            _phone = $"+7{DateTime.Now.ToString("mmssffffff")}";
        }

        public User Build()
        {
            var user =  new User(_email,_phone, _name, _hash);
            if (_totpSecret != null) user.SetTotpSecret(_totpSecret);
            if (isEmailVerified && _totpSecret != null) user.VerifyEmail();
            foreach (var role in _roles)
            {
                user.AssignRole(role);
            }
            return user;
        }

        public UserDb BuildDbUser()
        {
            var dbUser = new UserDb()
            {
                Id = Guid.NewGuid(),
                Email = _email,
                Phone = _phone,
                Name = _name,
                PasswordHash = _hash,
                Roles = _roles,
                TotpSecret = _totpSecret,
                IsEmailVerified = isEmailVerified
            };
            return dbUser;
        }

        public UserBuilder WithRoles(params string[] roles) 
        { 
            _roles.AddRange(roles);
            return this;
        }

        public UserBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public UserBuilder WithPhone(string phone)
        {
            _phone = phone;
            return this;
        }

        public UserBuilder WithPassword(string password, IPasswordHasher hasher)
        {
            var hash = hasher.GetHash(password);
            _hash = hash;
            return this;
        }

        public UserBuilder WithName(string username)
        {
            _name = username;
            return this;
        }
        public UserBuilder WithTotpSecret(string secret)
        {
            _totpSecret = secret;
            return this;
        }
        public UserBuilder WithVerifiedEmail() 
        {
            isEmailVerified = true;
            return this;
        }
    }
}
