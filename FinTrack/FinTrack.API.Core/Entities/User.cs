using FinTrack.API.Core.Common;
using FinTrack.API.Core.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
namespace FinTrack.API.Core.Entities
{
    /// <summary>
    /// Represents a registered user in the financial tracking system.
    /// </summary>
    /// <remarks>
    /// Responsibilities:
    /// <list type="bullet">
    /// <item>Authentication and identity in the system</item>
    /// <item>Personal information storage</item>
    /// <item>Ownership of the financial accounts</item>
    /// </list>
    /// </remarks>
    public class User : Entity
    {
        private const string phonePattern = @"^\+[1-9]\d{1,14}$";
        private const string emailPattern = @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";
        private const string hashPattern = @"^[A-Za-z0-9]+\.\d+\.[A-Za-z0-9+/]+={0,2}\.[A-Za-z0-9+/]+={0,2}$";

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

        /// <summary>
        /// Read-only collection with user's accounts
        /// </summary>
        public IReadOnlyCollection<Account> Accounts => accounts.AsReadOnly();

        /// <summary>
        /// User's name
        /// </summary>
        /// <remarks>
        /// Rules:
        /// <list type="bullet">
        /// <item>Must be non-empty and not whitespaces only</item>
        /// <item>
        /// Must be unique across the system
        /// (The uniqueness is provided by the database indexes)
        /// </item>
        /// <item>Must non-greater than 100 characters</item>
        /// </list>
        /// 
        /// Exceptions:
        /// <list type="bullet">
        /// <item><see cref="ArgumentException"/> - Incorrect name format</item>
        /// 
        /// </list>
        /// </remarks>

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

        /// <summary>
        /// User's email address
        /// </summary>
        /// <remarks>
        /// Rules:
        /// <list type="bullet">
        /// <item>Must follow RFC 5322</item>
        /// <item>
        /// Must be unique across the system
        /// (The uniqueness is provided by the database indexes)
        /// </item>
        /// </list>
        /// 
        /// Exceptions:
        /// <list type="bullet">
        /// <item><see cref="ArgumentException"/> - Incorrect email format</item>
        /// 
        /// </list>
        /// </remarks>
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

        /// <summary>
        /// User's phone
        /// </summary>
        /// <remarks>
        /// Rules:
        /// <list type="bullet">
        /// <item>Must follow E.164</item>
        /// <item>
        /// Must be unique across the system
        /// (The uniqueness is provided by the database indexes)
        /// </item>
        /// </list>
        /// 
        /// Exceptions:
        /// <list type="bullet">
        /// <item><see cref="ArgumentException"/> - Incorrect phone format</item>
        /// 
        /// </list>
        /// </remarks>
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

        /// <summary>
        /// User's password hash
        /// </summary>
        /// <remarks>
        /// Rules:
        /// <list type="bullet">
        /// <item>Must be result of cryptographic function</item>
        /// <item>
        /// Must follow next format:
        /// {Hash algorithm}.{Iteration count}.{Salt in Base64}.{Hash in Base64}
        /// </item>
        /// </list>
        /// 
        /// Exceptions:
        /// <list type="bullet">
        /// <item><see cref="ArgumentException"/> - Incorrect hash format</item>
        /// 
        /// </list>
        /// </remarks>
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

        /// <summary>
        /// Attaches user's account to account collection
        /// </summary>
        /// <param name="account">
        /// account of the current user
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// Account's reference to user and current user's id does not match
        /// </exception>
        public void AddAccount(Account account)
        {
            if (account.UserId != Id)
            {
                throw new AccountOwnershipException(Id, account.UserId);
            }
            accounts.Add(account);
        }

        /// <summary>
        /// Deattaches user's account from collection
        /// </summary>
        /// <param name="accountId">
        /// Id of the attached account
        /// </param>
        /// 
        /// <exception cref="KeyNotFoundException">
        /// Account with such id does not exist in user's collection
        /// </exception>
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
