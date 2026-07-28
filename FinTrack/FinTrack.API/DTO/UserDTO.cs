namespace FinTrack.API.DTO
{
    public readonly record struct CreateUserRequest(string Name, string Email, string Phone, string Password);
    public readonly record struct UpdateUserRequest(string? Name, string? Email, string? Phone);
}
