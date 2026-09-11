using System.Data;
using AlisPo.Api.Data;
using AlisPo.Api.DTOs;
using AlisPo.Api.Repositories;
using Dapper;
using AlisPo.Api.DTOs.Products;

namespace AlisPo.Api.Repositories;

/// <summary>
/// Product Repository
/// </summary>
public sealed class ProductRepository : IProductRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    private const string GetAllProductsSql = @"
SELECT
    p.ProductId         AS Id,
    p.ProductCode,
    p.ProductName       AS Name,
    ISNULL(p.ThaiName,'')      AS ThaiName,
    ot.OrderTypeName    AS MainCategory,
    c.CategoryName      AS SubCategory,
    u.UnitCode          AS Unit,
ISNULL(p.ImagePath,'')     AS Image,
p.AllowDecimal,
p.OutOfStock,
p.IsActive AS Active
FROM Products p
INNER JOIN Categories c
    ON p.CategoryId = c.CategoryId
INNER JOIN OrderTypes ot
    ON p.OrderTypeId = ot.OrderTypeId
LEFT JOIN Units u
    ON p.DefaultUnitId = u.UnitId
WHERE
    p.IsActive = 1
ORDER BY
    ot.DisplayOrder,
    c.DisplayOrder,
    p.DisplayOrder;";

    private const string GetOrderTypeIdSql = @"
SELECT OrderTypeId
FROM OrderTypes
WHERE OrderTypeName = @OrderTypeName
AND IsActive = 1;";

    private const string GetCategoryIdSql = @"
SELECT CategoryId
FROM Categories
WHERE
    OrderTypeId = @OrderTypeId
AND CategoryName = @CategoryName
AND IsActive = 1;";

    private const string GetUnitIdSql = @"
SELECT UnitId
FROM Units
WHERE
    UnitCode = @UnitCode
AND IsActive = 1;";

    private const string InsertProductSql = @"
INSERT INTO Products
(
    ProductCode,
    ProductName,
    ThaiName,
    OrderTypeId,
    CategoryId,
    DefaultUnitId,
    ImagePath,
    DisplayOrder,
    IsActive,
    AllowDecimal,
    OutOfStock
)
VALUES
(
    @ProductCode,
    @ProductName,
    @ThaiName,
    @OrderTypeId,
    @CategoryId,
    @DefaultUnitId,
@ImagePath,
0,
@IsActive,
@AllowDecimal,
@OutOfStock
);";

    private const string GetProductByIdSql = @"
SELECT
    p.ProductId         AS Id,
    p.ProductCode,
    p.ProductName       AS Name,
    ISNULL(p.ThaiName,'')      AS ThaiName,
    ot.OrderTypeName    AS MainCategory,
    c.CategoryName      AS SubCategory,
    u.UnitCode          AS Unit,
ISNULL(p.ImagePath,'')     AS Image,
p.AllowDecimal,
p.OutOfStock,
p.IsActive AS Active
FROM Products p
INNER JOIN Categories c
    ON p.CategoryId = c.CategoryId
INNER JOIN OrderTypes ot
    ON p.OrderTypeId = ot.OrderTypeId
LEFT JOIN Units u
    ON p.DefaultUnitId = u.UnitId
WHERE
    p.ProductId = @Id
AND p.IsActive = 1;";

    private const string ExistsSql = @"
SELECT COUNT(1)
FROM Products
WHERE ProductId = @Id
AND IsActive = 1;";

    private const string UpdateProductSql = @"
UPDATE Products
SET
    ProductCode = @ProductCode,
    ProductName = @ProductName,
    ThaiName = @ThaiName,
    OrderTypeId = @OrderTypeId,
    CategoryId = @CategoryId,
    DefaultUnitId = @DefaultUnitId,
ImagePath = @ImagePath,
AllowDecimal = @AllowDecimal,
OutOfStock = @OutOfStock,
IsActive = @IsActive,
UpdatedAt = GETUTCDATE()
WHERE ProductId = @Id;";

    private const string DeleteProductSql = @"
UPDATE Products
SET
    IsActive = 0,
    UpdatedAt = GETUTCDATE()
WHERE ProductId = @Id;";

    public ProductRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory
            ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: GetAllProductsSql,
            cancellationToken: cancellationToken,
            commandTimeout: 30);

        var products = await connection.QueryAsync<ProductDto>(command);

        return products;
    }
    public async Task AddAsync(
    CreateProductRequest request,
    CancellationToken cancellationToken = default)
    {
        using IDbConnection connection =
            _connectionFactory.CreateConnection();

        var orderTypeId =
            await connection.ExecuteScalarAsync<int?>(
                new CommandDefinition(
                    GetOrderTypeIdSql,
                    new
                    {
                        OrderTypeName = request.MainCategory
                    },
                    cancellationToken: cancellationToken));

        if (orderTypeId is null)
        {
            throw new Exception(
                $"OrderType '{request.MainCategory}' not found.");
        }

        var categoryId =
            await connection.ExecuteScalarAsync<int?>(
                new CommandDefinition(
                    GetCategoryIdSql,
                    new
                    {
                        OrderTypeId = orderTypeId,
                        CategoryName = request.SubCategory
                    },
                    cancellationToken: cancellationToken));

        if (categoryId is null)
        {
            throw new Exception(
                $"Category '{request.SubCategory}' not found.");
        }

        var unitId =
            await connection.ExecuteScalarAsync<int?>(
                new CommandDefinition(
                    GetUnitIdSql,
                    new
                    {
                        UnitCode = request.Unit
                    },
                    cancellationToken: cancellationToken));

        if (unitId is null)
        {
            throw new Exception(
                $"Unit '{request.Unit}' not found.");
        }

        await connection.ExecuteAsync(
            new CommandDefinition(
                InsertProductSql,
                new
                {
                    ProductCode = request.ProductCode,
                    ProductName = request.Name,
                    ThaiName = request.ThaiName,
                    OrderTypeId = orderTypeId,
                    CategoryId = categoryId,
                    DefaultUnitId = unitId,
                    ImagePath = request.Image,
                    AllowDecimal = request.AllowDecimal,
                    OutOfStock = request.OutOfStock,
                    IsActive = request.Active
                },
                cancellationToken: cancellationToken));
    }
    public async Task<ProductDto?> GetByIdAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        using IDbConnection connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: GetProductByIdSql,
            parameters: new { Id = id },
            cancellationToken: cancellationToken,
            commandTimeout: 30);

        return await connection.QuerySingleOrDefaultAsync<ProductDto>(command);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using IDbConnection connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                DeleteProductSql,
                new { Id = id },
                cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        using IDbConnection connection =
            _connectionFactory.CreateConnection();

        var orderTypeId =
            await connection.ExecuteScalarAsync<int?>(
                new CommandDefinition(
                    GetOrderTypeIdSql,
                    new { OrderTypeName = request.MainCategory },
                    cancellationToken: cancellationToken));

        if (orderTypeId is null)
            throw new Exception($"OrderType '{request.MainCategory}' not found.");

        var categoryId =
            await connection.ExecuteScalarAsync<int?>(
                new CommandDefinition(
                    GetCategoryIdSql,
                    new
                    {
                        OrderTypeId = orderTypeId,
                        CategoryName = request.SubCategory
                    },
                    cancellationToken: cancellationToken));

        if (categoryId is null)
            throw new Exception($"Category '{request.SubCategory}' not found.");

        var unitId =
            await connection.ExecuteScalarAsync<int?>(
                new CommandDefinition(
                    GetUnitIdSql,
                    new { UnitCode = request.Unit },
                    cancellationToken: cancellationToken));

        if (unitId is null)
            throw new Exception($"Unit '{request.Unit}' not found.");

        await connection.ExecuteAsync(
            new CommandDefinition(
                UpdateProductSql,
                new
                {
                    Id = id,
                    ProductCode = request.ProductCode,
                    ProductName = request.Name,
                    ThaiName = request.ThaiName,
                    OrderTypeId = orderTypeId,
                    CategoryId = categoryId,
                    DefaultUnitId = unitId,
                    ImagePath = request.Image,
                    AllowDecimal = request.AllowDecimal,
                    OutOfStock = request.OutOfStock,
                    IsActive = request.Active
                },
                cancellationToken: cancellationToken));
    }
    public async Task<bool> ExistsAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        using IDbConnection connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: ExistsSql,
            parameters: new { Id = id },
            cancellationToken: cancellationToken,
            commandTimeout: 30);

        var count = await connection.ExecuteScalarAsync<int>(command);

        return count > 0;
    }
}