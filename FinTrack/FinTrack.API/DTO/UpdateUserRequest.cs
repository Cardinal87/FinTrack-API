using System.ComponentModel.DataAnnotations;

namespace FinTrack.API.DTO
{
    public class UpdateUserRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
