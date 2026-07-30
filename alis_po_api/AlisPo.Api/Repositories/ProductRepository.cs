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
    p.AllowDecimal
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
    p.DisplayOrder,
    p.ProductName;";

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
    AllowDecimal
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
    @AllowDecimal
);";

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
                    IsActive = request.Active
                },
                cancellationToken: cancellationToken));
    }
}