using Dapper;
using Users.Application.Database;
using Users.Application.Models;

namespace Users.Application.Repository;

public class UsersRepository : IUsersRepository
{
    // private readonly UsersDb _usersDb;
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public UsersRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }


    public async Task<bool> CreateUserAsync(User user, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(
            new CommandDefinition("""
                                  insert into users (id, username, email, password, is_admin, created_at, updated_at)
                                  values (@Id, @Username, @Email, @Password, @IsAdmin, @CreatedAt, @UpdatedAt)
                                  """, user, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateUserAsync(Guid id, User user, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(
            new CommandDefinition("""
                                  update users set username = @Username, email = @Email, updated_at = @UpdatedAt
                                  where id = Id
                                  """, user, cancellationToken: token));

        return result > 0;

    }

    public async Task<bool> DeleteUserAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(
            new CommandDefinition("""
                                  delete from users where id = @id
                                  """, new { id }, cancellationToken: token));
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<User?> GetUserByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var user = await connection.QuerySingleOrDefaultAsync<User>(
            new CommandDefinition("""
                                  select * from users where id = @id
                                  """, new { id }, cancellationToken: token));
        if (user == null) return null;

        return user;
    }

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var user = await connection.QuerySingleOrDefaultAsync<User>(
            new CommandDefinition("""
                                  select * from users where username = @username
                                  """, new { username }, cancellationToken: token));
        if (user == null) return null;

        return user;
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var user = await connection.QuerySingleOrDefaultAsync<User>(
            new CommandDefinition("""
                                  select * from users where email = @email
                                  """, new { email }, cancellationToken: token));
        if (user == null) return null;

        return user;
    }

    public async Task<IEnumerable<User>> GetUsersAsync(CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        var result = await connection.QueryAsync<User>(
            new CommandDefinition("""
                                  Select * from users
                                  """, cancellationToken: token));

        return result.Select(x => new User()
        {
            Id = x.Id,
            Username = x.Username,
            Email = x.Email,
            Password = x.Password,
            IsAdmin = x.IsAdmin,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        });
    }

    public async Task<bool> ExistUserByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition("""
                                  Select count(1) from users where id = @id
                                  """, new { id }, cancellationToken: token));

        return result;
    }

    public async Task<bool> UpdatePasswordAsync(Guid id, string passwordHash, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        
        var result = await connection.ExecuteAsync(
            new CommandDefinition(
            
                """
                UPDATE users
                SET password = @passwordHash
                WHERE id = @id
                """, new { id, passwordHash }, cancellationToken: token));
        
        return result > 0;
    }
}