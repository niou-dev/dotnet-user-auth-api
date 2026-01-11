using Users.Contracts.Auth;

namespace Users.Application.Services;

public interface IAuthService
{
    Task<SignupResponse> SignUp(SignUpRequest signUpRequest);
    Task<LoginResponse> Login(LoginRequest loginRequest);
}