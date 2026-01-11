namespace Users.Contracts.Users;

public class UserRequest
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}