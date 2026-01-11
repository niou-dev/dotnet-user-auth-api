using Dapper;

namespace Users.Application.Database;

public class DbInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DbInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        await connection.ExecuteAsync(
            """
            create table if not exists users(
                id UUID primary key,
                username TEXT not null unique,
                email TEXT not null unique,
                password text not null,
                is_admin boolean not null default false,
                created_at timestamptz not null,
                updated_at timestamptz not null
                );
            """
            );
    }
}