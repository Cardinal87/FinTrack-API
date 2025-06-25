using FinTrack.API.Core.Common;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
namespace FinTrack.API.Core.Entities
{

    public class User : Entity
    {
        private const string phonePattern = @"^\+[1-9]\d{1,14}$";
        private const string emailPattern = @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";
        private const string hashPattern = @"^[0-9a-fA-F]{64}$";

        private string name;
        private string email;
        private string phone;
        private string passwordHash;
        private readonly List<Account> accounts = new();

        public User(string email, string phone, string name, string hash)
        {
            Email = email;
            Phone = phone;
            Name = name;
            PasswordHash = hash;
        }


        public IReadOnlyCollection<Account> Accounts => accounts.AsReadOnly();


        public string Name { 
            get => name; 
            
            [MemberNotNull(nameof(name))]
            set
            {
                if (String.IsNullOrWhiteSpace(value) || value.Length > 100)
                {
                    throw new ArgumentException("Incorrect name format");
                }
                name = value;
            }
        }
        public string Email 
        { 
            get => email;
            
            [MemberNotNull(nameof(email))]
            set 
            {
                if (!Regex.IsMatch(value, emailPattern))
                {
                    throw new ArgumentException("Incorrect email format");
                }
                email = value;
            } 
        }
        public string Phone 
        { 
            get => phone;
            [MemberNotNull(nameof(phone))]
            set
            {
                if (!Regex.IsMatch(value, phonePattern))
                {
                    throw new ArgumentException("Incorrect phone format");
                }
                phone = value;
            }
        }
        public string PasswordHash
        {
            get => passwordHash;
            [MemberNotNull(nameof(passwordHash))]
            set
            {
                if (!Regex.IsMatch(value, hashPattern))
                {
                    throw new ArgumentException("Incorrect hash format");
                }
                passwordHash = value;
            }
        }

        public void AddAccount(Account account)
        {
            if (account.UserId != Id)
            {
                throw new ArgumentException("Account does not belong to the user");
            }
            accounts.Add(account);
        }

        public void DeleteAccount(Guid accountId)
        {
            var account = accounts.FirstOrDefault(t => t.Id == accountId);
            if (account == null)
            {
                throw new KeyNotFoundException("Account with this id does not belong to the user");
            }
            accounts.Remove(account);
        }

    }
}
