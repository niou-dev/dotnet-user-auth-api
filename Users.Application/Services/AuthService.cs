using Microsoft.AspNetCore.Identity;
using Users.Application.Database;
using Users.Application.Models;
using Users.Application.Repository;
using Users.Contracts.Auth;
using Users.Contracts.Users;

namespace Users.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(IUsersRepository usersRepository, IJwtService jwtService, IPasswordHasher<User> passwordHasher)
    {
        _usersRepository = usersRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    public async Task<SignupResponse> SignUp(SignUpRequest signUpRequest)
    {
        var result = await _usersRepository.GetUserByUsernameAsync(signUpRequest.Username);
        if (!(result == null))
        {
            
            throw new InvalidOperationException("Username already exists");
        }
        
        User user = new User()
        {
            Id = Guid.NewGuid(),
            Username = signUpRequest.Username,
            Password = "",
            Email = signUpRequest.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        user.Password = _passwordHasher.HashPassword(user, signUpRequest.Password);
        
        var succededCreation = await _usersRepository.CreateUserAsync(user);
        if (!succededCreation) throw new InvalidOperationException("Could not create user");
        
        var jwt = _jwtService.GenerateToken(user);
        SignupResponse response = new SignupResponse();
        response.UserResponse = new UserResponse()
        {
            Id =  user.Id,
            Username = user.Username,
            Email = user.Email
        };
        response.Jwt = jwt;
        
        return response;
    }

    public async Task<LoginResponse> Login(LoginRequest loginRequest)
    {
        var result = await _usersRepository.GetUserByUsernameAsync(loginRequest.Username);
        if (result == null) throw new InvalidOperationException("Invalid credentials");

        User user = result;
        
        var correctPassword = _passwordHasher.VerifyHashedPassword(user, user.Password, loginRequest.Password);
        
        if (correctPassword == PasswordVerificationResult.Failed) throw new InvalidOperationException("Invalid credentials");
     
        var customClaims = new Dictionary<string, object>();
        customClaims["admin"] = user.IsAdmin;
        
        return new LoginResponse()
        {
            
            Jwt = _jwtService.GenerateToken(user, customClaims),
            UserResponse = new UserResponse() { Id = user.Id, Username = user.Username,  Email = user.Email }
        };
    }
}