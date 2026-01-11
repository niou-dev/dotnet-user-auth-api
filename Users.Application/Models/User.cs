namespace Users.Application.Models;

public class User
{
    public Guid Id { get; init; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public bool IsAdmin { get; set; } = false;
}