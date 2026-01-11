using Microsoft.AspNetCore.Identity;
using Users.Application.Models;
using Users.Contracts.Users;

namespace Users.Application.Mapping;

public static class ContractMapping
{
    
    public static User MapToUser(this CreateUserRequest createUserRequest)
    {
        User user = new User
        {
            Id = Guid.NewGuid(),
            Username = createUserRequest.Username,
            Password = "",
            Email = createUserRequest.Email,
            IsAdmin = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        return user;
    }

    public static UserResponse MapToUserResponse(this User user)
    {
        return new UserResponse()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }

    /*public static User MapToUser(this UpdateUserRequest updateUserRequest, Guid id)
    {
        return new User
        {
            Id = id,
            Username = updateUserRequest.Username,
            Email = updateUserRequest.Email,
            
        }
            
    }*/
}