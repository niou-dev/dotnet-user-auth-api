using Users.Contracts.Users;

namespace Users.Application.Services;

public interface IUsersService
{
    public Task<bool> CreateAdmin(CreateUserRequest request);
    public Task<bool> CreateUser(CreateUserRequest request);
    public Task<bool> DeleteUser(Guid id);
    
    public Task<UserResponse?> UpdateUser(Guid id, UpdateUserRequest request);
    
    public Task<UserResponse?> GetUserById(Guid id);
    public Task<UserResponse?> GetUserByUsername(string username);
    public Task<UserResponse?> GetUserByEmail(string email);
    
    public Task<IEnumerable<UserResponse>> GetUsers();
}