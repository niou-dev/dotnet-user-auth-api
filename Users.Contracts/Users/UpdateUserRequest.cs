namespace Users.Contracts.Users;

public class UpdateUserRequest
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}