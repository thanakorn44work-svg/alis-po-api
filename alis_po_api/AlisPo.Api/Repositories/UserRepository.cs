using AlisPo.Api.Data;
using AlisPo.Api.Repositories.Interfaces;
using Dapper;

namespace AlisPo.Api.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public UserRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory
            ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<UserInfo?> GetByBranchIdAsync(
        int branchId,
        CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        const string sql = """
            SELECT TOP 1
                UserId,
                Username,
                BranchId
            FROM dbo.Users
            WHERE BranchId = @BranchId
              AND IsActive = 1
            ORDER BY UserId;
            """;

        return await connection.QuerySingleOrDefaultAsync<UserInfo>(
            new CommandDefinition(
                sql,
                new { BranchId = branchId },
                cancellationToken: cancellationToken));
    }
}