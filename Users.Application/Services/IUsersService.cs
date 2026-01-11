using Users.Contracts.Users;

namespace Users.Application.Services;

public interface IUsersService
{
    public Task<bool> CreateAdminAsync(CreateUserRequest request);
    public Task<bool> CreateUserAsync(CreateUserRequest request);
    public Task<bool> DeleteUserAsync(Guid id);
    
    public Task<UserResponse?> UpdateUserAsync(Guid id, UpdateUserRequest request);
    
    public Task<UserResponse?> GetUserByIdAsync(Guid id);
    public Task<UserResponse?> GetUserByUsernameAsync(string username);
    public Task<UserResponse?> GetUserByEmailAsync(string email);
    
    public Task<IEnumerable<UserResponse>> GetUsersAsync();
    
    public Task<bool> ChangePasswordAsync(Guid id, ChangePasswordRequest request, CancellationToken token);
}