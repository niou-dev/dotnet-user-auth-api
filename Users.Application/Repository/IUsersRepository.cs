using Users.Application.Models;

namespace Users.Application.Repository;

public interface IUsersRepository
{
    Task<bool> CreateUserAsync(User user, CancellationToken token = default);

    Task<bool> UpdateUserAsync(Guid id, User user, CancellationToken token = default);

    Task<bool> DeleteUserAsync(Guid id, CancellationToken token = default);

    Task<User?> GetUserByIdAsync(Guid id, CancellationToken token = default);

    Task<User?> GetUserByUsernameAsync(string username, CancellationToken token = default);

    Task<User?> GetUserByEmailAsync(string email, CancellationToken token = default);

    Task<IEnumerable<User>> GetUsersAsync(CancellationToken token = default);

    Task<bool> ExistUserByIdAsync(Guid id, CancellationToken token = default);
}