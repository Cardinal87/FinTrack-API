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
        private static readonly Regex phonePattern = new Regex(@"^\+[1-9]\d{1,14}$", RegexOptions.Compiled);
        private static readonly Regex emailPattern = new Regex(@"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$",
                                                    RegexOptions.Compiled);
        private static readonly Regex hashPattern = new Regex(@"^[A-Za-z0-9]+\.\d+\.[A-Za-z0-9+/]+={0,2}\.[A-Za-z0-9+/]+={0,2}$", RegexOptions.Compiled);
        private static readonly Regex base32Pattern = new Regex(@"^(?:[A-Z2-7]{8})*(?:[A-Z2-7]{2}={6}|[A-Z2-7]{4}={4}|[A-Z2-7]{5}={3}|[A-Z2-7]{7}=)?$",
                                                    RegexOptions.Compiled);

        private string name;
        private string email;
        private string phone;
        private string passwordHash;
        private string? totpSecret;
        private bool isEmailVerified;
        private readonly List<string> roles = new();

        public User(string email, string phone, string name, string hash)
        {
            Email = email;
            Phone = phone;
            Name = name;
            PasswordHash = hash;
            isEmailVerified = false;
        }


        /// <summary>
        /// Read-only collection with user's roles
        /// </summary>
        public IReadOnlyCollection<string> Roles => roles.AsReadOnly();

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
                if (!emailPattern.IsMatch(value))
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
                if (!phonePattern.IsMatch(value))
                {
                    throw new ArgumentException("Incorrect phone format");
                }
                phone = value;
            }
        }

        /// <summary>
        /// Indicates whether the user's email is verified
        /// </summary>
        public bool IsEmailVerified 
        {
            get => isEmailVerified;
        }

        /// <summary>
        /// Returns the TOTP Secret for current user
        /// </summary>
        public string? TotpSecret {
            get => totpSecret;
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
                if (!hashPattern.IsMatch(value))
                {
                    throw new ArgumentException("Incorrect hash format");
                }
                passwordHash = value;
            }
        }


        /// <summary>
        /// Assings a new role to user
        /// </summary>
        /// <param name="role">one of the string constants of <see cref="UserRoles"/></param>
        /// <exception cref="DomainException">role not defined in <see cref="UserRoles"/></exception>
        public void AssignRole(string role)
        {
            if (!UserRoles.AllRoles.Contains(role))
            {
                throw new DomainException($"incorrect role {role}");
            }
            if (!roles.Contains(role))
            {
                roles.Add(role);
            }
        }

        /// <summary>
        /// Verifies user email
        /// </summary>
        /// <exception cref="DomainException"><see cref="TotpSecret"/> is null</exception>
        public void VerifyEmail()
        {
            if (totpSecret == null)
            {
                throw new DomainException("TOTP Secret is null. Email cannot be verified without TOTP Secret");
            }
            isEmailVerified = true;
        }

        /// <summary>
        /// Sets TOTP Secret for current user
        /// </summary>
        /// <param name="secret">TOTP Secret in Base32 format</param>
        /// <exception cref="ArgumentException">Invalid format of <paramref name="secret"/></exception>
        public void SetTotpSecret(string secret)
        {
            if (!base32Pattern.IsMatch(secret))
            {
                throw new ArgumentException("Invalid TOTP Secret Format. Must match base32 string format");
            }
            totpSecret = secret;
        }
    }
}
