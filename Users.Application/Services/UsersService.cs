using System.Runtime.InteropServices.JavaScript;
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


    public async Task<bool> CreateUser(CreateUserRequest request)
    {
        User user = request.MapToUser();
        user.Password = _passwordHasher.HashPassword(user, request.Password);
        
        return await _usersRepository.CreateUserAsync(user);
    }

    public async Task<bool> DeleteUser(Guid id)
    {
        return  await _usersRepository.DeleteUserAsync(id);
    }

    public async Task<UserResponse?> UpdateUser(Guid id, UpdateUserRequest request)
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

    public async Task<UserResponse?> GetUserById(Guid id)
    {
        var result = await _usersRepository.GetUserByIdAsync(id);

        if (result == null) return null;
        
        return result.MapToUserResponse();
    }

    public async Task<UserResponse?> GetUserByUsername(string username)
    {
        var result = await _usersRepository.GetUserByUsernameAsync(username);

        if (result == null) return null;
        
        return result.MapToUserResponse();
        
    }

    public async Task<UserResponse?> GetUserByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<UserResponse>> GetUsers()
    {
        var result = await _usersRepository.GetUsersAsync();

        return result.Select(x => x.MapToUserResponse());
    }

    public Task<bool> CreateAdmin(CreateUserRequest request)
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
}