namespace Users.Contracts.Users;

public class UserResponse
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
}