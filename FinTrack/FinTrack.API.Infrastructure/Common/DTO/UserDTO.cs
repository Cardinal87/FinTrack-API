using FinTrack.API.Core.Entities;

namespace FinTrack.API.Infrastructure.Common.DTO
{
    public class UserDTO
    {
        public UserDTO() { }
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public List<string> Roles { get; set; } = new();

    }
}
