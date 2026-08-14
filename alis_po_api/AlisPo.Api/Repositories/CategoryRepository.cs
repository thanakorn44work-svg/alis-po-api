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
    ISNULL(c.DisplayOrder, 0) AS SubDisplayOrder
FROM OrderTypes ot
LEFT JOIN Categories c
    ON c.OrderTypeId = ot.OrderTypeId
    AND c.IsActive = 1
WHERE ot.IsActive = 1
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

    public async Task AddSubCategoryAsync(
    string mainCategory,
    string subCategory,
    CancellationToken cancellationToken = default)
    {
        using IDbConnection connection =
            _connectionFactory.CreateConnection();

        var orderTypeId = await connection.ExecuteScalarAsync<int?>(
            new CommandDefinition(
                commandText: @"
SELECT OrderTypeId
FROM OrderTypes
WHERE OrderTypeName = @MainCategory
  AND IsActive = 1;",
                parameters: new
                {
                    MainCategory = mainCategory
                },
                cancellationToken: cancellationToken,
                commandTimeout: 30));

        if (orderTypeId is null)
        {
            throw new InvalidOperationException(
                $"Order type '{mainCategory}' not found.");
        }

        var exists = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                commandText: @"
SELECT COUNT(1)
FROM Categories
WHERE OrderTypeId = @OrderTypeId
  AND CategoryName = @SubCategory
  AND IsActive = 1;",
                parameters: new
                {
                    OrderTypeId = orderTypeId.Value,
                    SubCategory = subCategory
                },
                cancellationToken: cancellationToken,
                commandTimeout: 30));

        if (exists > 0)
        {
            throw new InvalidOperationException(
                $"Sub category '{subCategory}' already exists.");
        }

        var displayOrder = await connection.ExecuteScalarAsync<int?>(
            new CommandDefinition(
                commandText: @"
SELECT ISNULL(MAX(DisplayOrder), 0) + 1
FROM Categories
WHERE OrderTypeId = @OrderTypeId;",
                parameters: new
                {
                    OrderTypeId = orderTypeId.Value
                },
                cancellationToken: cancellationToken,
                commandTimeout: 30));

        await connection.ExecuteAsync(
            new CommandDefinition(
                commandText: @"
INSERT INTO Categories
(
    CategoryCode,
    OrderTypeId,
    CategoryName,
    DisplayOrder,
    IsActive
)
VALUES
(
    @CategoryCode,
    @OrderTypeId,
    @SubCategory,
    @DisplayOrder,
    1
);",
                parameters: new
                {
                    CategoryCode = subCategory
        .Trim()
        .ToUpperInvariant()
        .Replace(" ", "_")
        .Replace("&", ""),
                    OrderTypeId = orderTypeId.Value,
                    SubCategory = subCategory,
                    DisplayOrder = displayOrder ?? 1
                },
                cancellationToken: cancellationToken,
                commandTimeout: 30));
    }
    public async Task AddMainCategoryAsync(
    string mainCategory,
    CancellationToken cancellationToken = default)
    {
        using IDbConnection connection =
            _connectionFactory.CreateConnection();

        var name = mainCategory.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Main category name is required.",
                nameof(mainCategory));
        }

        var exists = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                commandText: @"
SELECT COUNT(1)
FROM OrderTypes
WHERE OrderTypeName = @OrderTypeName
  AND IsActive = 1;",
                parameters: new
                {
                    OrderTypeName = name
                },
                cancellationToken: cancellationToken,
                commandTimeout: 30));

        if (exists > 0)
        {
            throw new InvalidOperationException(
                $"Main category '{name}' already exists.");
        }

        var orderTypeCode = name
            .ToUpperInvariant()
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("_", "");

        var codeExists = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                commandText: @"
SELECT COUNT(1)
FROM OrderTypes
WHERE OrderTypeCode = @OrderTypeCode;",
                parameters: new
                {
                    OrderTypeCode = orderTypeCode
                },
                cancellationToken: cancellationToken,
                commandTimeout: 30));

        if (codeExists > 0)
        {
            throw new InvalidOperationException(
                $"Order type code '{orderTypeCode}' already exists.");
        }

        var displayOrder = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                commandText: @"
SELECT ISNULL(MAX(DisplayOrder), 0) + 1
FROM OrderTypes;",
                cancellationToken: cancellationToken,
                commandTimeout: 30));

        await connection.ExecuteAsync(
            new CommandDefinition(
                commandText: @"
INSERT INTO OrderTypes
(
    OrderTypeCode,
    OrderTypeName,
    DisplayOrder,
    IsActive,
    CreatedAt
)
VALUES
(
    @OrderTypeCode,
    @OrderTypeName,
    @DisplayOrder,
    1,
    GETDATE()
);",
                parameters: new
                {
                    OrderTypeCode = orderTypeCode,
                    OrderTypeName = name,
                    DisplayOrder = displayOrder
                },
                cancellationToken: cancellationToken,
                commandTimeout: 30));
    }
    public async Task DeleteSubCategoryAsync(
    string mainCategory,
    string subCategory,
    CancellationToken cancellationToken = default)
    {
        using IDbConnection connection =
            _connectionFactory.CreateConnection();

        const string sql = @"
UPDATE c
SET
    c.IsActive = 0
FROM Categories c
INNER JOIN OrderTypes ot
    ON c.OrderTypeId = ot.OrderTypeId
WHERE
    ot.OrderTypeName = @MainCategory
    AND c.CategoryName = @SubCategory
    AND c.IsActive = 1;";

        await connection.ExecuteAsync(
            new CommandDefinition(
                commandText: sql,
                parameters: new
                {
                    MainCategory = mainCategory.Trim(),
                    SubCategory = subCategory.Trim()
                },
                cancellationToken: cancellationToken,
                commandTimeout: 30));
    }
    public async Task DeleteMainCategoryAsync(
    string mainCategory,
    CancellationToken cancellationToken = default)
    {
        using IDbConnection connection =
            _connectionFactory.CreateConnection();

        const string sql = @"
UPDATE OrderTypes
SET
    IsActive = 0
WHERE
    OrderTypeName = @MainCategory
    AND IsActive = 1;";

        await connection.ExecuteAsync(
            new CommandDefinition(
                commandText: sql,
                parameters: new
                {
                    MainCategory = mainCategory.Trim()
                },
                cancellationToken: cancellationToken,
                commandTimeout: 30));
    }
    public async Task RenameSubCategoryAsync(
    string mainCategory,
    string oldName,
    string newName,
    CancellationToken cancellationToken = default)
    {
        using IDbConnection connection =
            _connectionFactory.CreateConnection();

        const string sql = @"
UPDATE c
SET
    c.CategoryName = @NewName
FROM Categories c
INNER JOIN OrderTypes ot
    ON c.OrderTypeId = ot.OrderTypeId
WHERE
    ot.OrderTypeName = @MainCategory
    AND c.CategoryName = @OldName
    AND c.IsActive = 1;";

        await connection.ExecuteAsync(
            new CommandDefinition(
                commandText: sql,
                parameters: new
                {
                    MainCategory = mainCategory.Trim(),
                    OldName = oldName.Trim(),
                    NewName = newName.Trim()
                },
                cancellationToken: cancellationToken,
                commandTimeout: 30));
    }
    public async Task RenameMainCategoryAsync(
        string oldName,
        string newName,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
        UPDATE dbo.OrderTypes
        SET OrderTypeName = @NewName
        WHERE OrderTypeName = @OldName;
        """;

        using IDbConnection connection =
            _connectionFactory.CreateConnection();

        if (connection is Microsoft.Data.SqlClient.SqlConnection sqlConnection)
        {
            await sqlConnection.OpenAsync(cancellationToken);
        }

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    OldName = oldName,
                    NewName = newName
                },
                cancellationToken: cancellationToken));
    }
}