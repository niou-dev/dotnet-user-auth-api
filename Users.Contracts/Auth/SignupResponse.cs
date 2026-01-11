using Users.Contracts.Users;

namespace Users.Contracts.Auth;

public class SignupResponse
{
    public UserResponse UserResponse { get; set; }
    public string Jwt { get; set; }
}