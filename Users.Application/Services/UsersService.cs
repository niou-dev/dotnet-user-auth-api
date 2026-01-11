using System.Security.Authentication;
using Microsoft.AspNetCore.Identity;
using Users.Application.Mapping;
using Users.Application.Models;
using Users.Application.Repository;
using Users.Contracts.Users;

namespace Users.Application.Services;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UsersService(IUsersRepository usersRepository, IPasswordHasher<User> passwordHasher)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
    }


    public async Task<bool> CreateUserAsync(CreateUserRequest request)
    {
        User user = request.MapToUser();
        user.Password = _passwordHasher.HashPassword(user, request.Password);
        
        return await _usersRepository.CreateUserAsync(user);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        return  await _usersRepository.DeleteUserAsync(id);
    }

    public async Task<UserResponse?> UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        var userExists = await _usersRepository.ExistUserByIdAsync(id);
        if (userExists) return null;
        
        User? user = await  _usersRepository.GetUserByIdAsync(id);
        
        if (user == null) return null;
        
        user.Username = request.Username;
        user.Email = request.Email;
        user.UpdatedAt = DateTime.UtcNow;
        
        await _usersRepository.UpdateUserAsync(id, user);
        
        return user.MapToUserResponse();
    }

    public async Task<UserResponse?> GetUserByIdAsync(Guid id)
    {
        var result = await _usersRepository.GetUserByIdAsync(id);

        if (result == null) return null;
        
        return result.MapToUserResponse();
    }

    public async Task<UserResponse?> GetUserByUsernameAsync(string username)
    {
        var result = await _usersRepository.GetUserByUsernameAsync(username);

        if (result == null) return null;
        
        return result.MapToUserResponse();
        
    }

    public async Task<UserResponse?> GetUserByEmailAsync(string email)
    {
        var user = await _usersRepository.GetUserByEmailAsync(email);
        if (user == null) return null;
        return user.MapToUserResponse();
    }

    public async Task<IEnumerable<UserResponse>> GetUsersAsync()
    {
        var result = await _usersRepository.GetUsersAsync();

        return result.Select(x => x.MapToUserResponse());
    }

    public Task<bool> CreateAdminAsync(CreateUserRequest request)
    {
        User user = new User()
        {
            Id = Guid.NewGuid(),
            Username =  request.Username,
            Email =  request.Email,
            Password = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsAdmin = true
        };
        
        user.Password = _passwordHasher.HashPassword(user, request.Password);
        return _usersRepository.CreateUserAsync(user);
    }

    public async Task<bool> ChangePasswordAsync(Guid id, ChangePasswordRequest request, CancellationToken token)
    {
        var user = await _usersRepository.GetUserByIdAsync(id, token);
        if (user is null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var passwordVerify = _passwordHasher.VerifyHashedPassword(user, user.Password, request.OldPassword);
        if (passwordVerify == PasswordVerificationResult.Failed)
        {
            throw new InvalidCredentialException();
        }
        var newHashed = _passwordHasher.HashPassword(user, request.NewPassword);
        await _usersRepository.UpdatePasswordAsync(id, newHashed, token);
        return true;
    }
}