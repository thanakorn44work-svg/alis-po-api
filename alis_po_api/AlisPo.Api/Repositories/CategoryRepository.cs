using System.Data;
using AlisPo.Api.Data;
using AlisPo.Api.DTOs;
using AlisPo.Api.Repositories.Interfaces;
using Dapper;

namespace AlisPo.Api.Repositories;

/// <summary>
/// Repository สำหรับจัดการข้อมูลหมวดหมู่สินค้า
/// </summary>
public sealed class CategoryRepository : ICategoryRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    private const string GetAllCategoriesSql = @"
SELECT
    ot.OrderTypeName  AS MainCategory,
    c.CategoryName    AS SubCategory,
    ot.DisplayOrder   AS MainDisplayOrder,
    c.DisplayOrder    AS SubDisplayOrder
FROM Categories c
INNER JOIN OrderTypes ot
    ON c.OrderTypeId = ot.OrderTypeId
ORDER BY
    ot.DisplayOrder,
    c.DisplayOrder;";

    public CategoryRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory
            ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: GetAllCategoriesSql,
            cancellationToken: cancellationToken,
            commandTimeout: 30);

        return await connection.QueryAsync<CategoryDto>(command);
    }
}