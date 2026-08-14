using System.Text.Json;
using AlisPo.Api.Data;
using AlisPo.Api.Models;
using Dapper;

namespace AlisPo.Api.Seeders;

public sealed class ProductSeeder
{
    private readonly SqlConnectionFactory _connectionFactory;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // DEV MainCategory → Production SupplierName
    // null = บริษัทไม่มี Supplier สำหรับ Order Type นี้
    private static readonly IReadOnlyDictionary<string, string?> SupplierMap =
        new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        {
            ["Makro Order"] = "Makro",
            ["Store Order"] = "ALI'S Warehouse",
            ["Kanna Juice Order"] = "KANNA HEALTHY CO.,LTD.",
            ["Fruit Order"] = null,
            ["Bread Order"] = "Ali Bakery ( Bread Yossef )",
            ["Cake Order"] = "Cake",
            ["Supercheap Order"] = null
        };

    public ProductSeeder(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task SeedAsync()
    {
        var file = Path.Combine(
            AppContext.BaseDirectory,
            "Imports",
            "products.json");

        if (!File.Exists(file))
            throw new FileNotFoundException(
                "products.json not found.",
                file);

        var products = JsonSerializer.Deserialize<List<ProductImport>>(
            await File.ReadAllTextAsync(file),
            JsonOptions) ?? [];

        if (products.Count == 0)
        {
            Console.WriteLine("No products found.");
            return;
        }

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var transaction = connection.BeginTransaction();

        try
        {
            var imported = 0;
            var skipped = 0;

            Console.WriteLine($"Loading {products.Count} products...");

            foreach (var item in products)
            {
                var exists = await connection.ExecuteScalarAsync<int>(
                    """
                    SELECT COUNT(*)
                    FROM Products
                    WHERE ProductCode = @Code
                    """,
                    new
                    {
                        Code = item.Id
                    },
                    transaction);

                if (exists > 0)
                {
                    skipped++;
                    continue;
                }

                // Order Type
                var orderTypeId = await connection.ExecuteScalarAsync<int?>(
                    """
                    SELECT OrderTypeId
                    FROM OrderTypes
                    WHERE OrderTypeName = @Name
                    """,
                    new
                    {
                        Name = item.MainCategory
                    },
                    transaction)
                    ?? throw new Exception(
                        $"OrderType not found: {item.MainCategory}");

                // Category
                var categoryId = await connection.ExecuteScalarAsync<int?>(
                    """
                    SELECT CategoryId
                    FROM Categories
                    WHERE OrderTypeId = @OrderTypeId
                      AND CategoryName = @CategoryName
                    """,
                    new
                    {
                        OrderTypeId = orderTypeId,
                        CategoryName = item.SubCategory
                    },
                    transaction)
                    ?? throw new Exception(
                        $"Category not found: {item.SubCategory}");

                // Unit
                var unitId = await connection.ExecuteScalarAsync<int?>(
                    """
                    SELECT UnitId
                    FROM Units
                    WHERE UnitCode = @Code
                    """,
                    new
                    {
                        Code = item.Unit
                    },
                    transaction)
                    ?? throw new Exception(
                        $"Unit not found: {item.Unit}");

                // Supplier
                int? supplierId = null;

                if (SupplierMap.TryGetValue(
                        item.MainCategory,
                        out var supplierName)
                    && supplierName is not null)
                {
                    supplierId = await connection.ExecuteScalarAsync<int?>(
                        """
                        SELECT SupplierID
                        FROM Suppliers
                        WHERE SupplierName = @SupplierName
                          AND IsActive = 1
                        """,
                        new
                        {
                            SupplierName = supplierName
                        },
                        transaction);

                    if (supplierId is null)
                    {
                        throw new Exception(
                            $"Supplier not found: {supplierName}");
                    }
                }

                // Insert Product
                var productId = await connection.ExecuteScalarAsync<int>(
                    """
                    INSERT INTO Products
                    (
                        ProductCode,
                        ProductName,
                        ThaiName,
                        OrderTypeId,
                        CategoryId,
                        SupplierId,
                        ImagePath,
                        Description,
                        DisplayOrder,
                        IsActive,
                        CreatedAt,
                        UpdatedAt,
                        AllowDecimal,
                        DefaultUnitId,
                        OutOfStock
                    )
                    OUTPUT INSERTED.ProductId
                    VALUES
                    (
                        @ProductCode,
                        @ProductName,
                        @ThaiName,
                        @OrderTypeId,
                        @CategoryId,
                        @SupplierId,
                        @ImagePath,
                        '',
                        0,
                        @IsActive,
                        GETDATE(),
                        GETDATE(),
                        @AllowDecimal,
                        @DefaultUnitId,
                        @OutOfStock
                    )
                    """,
                    new
                    {
                        ProductCode = item.Id,
                        ProductName = item.Name,
                        ThaiName = item.ThaiName,
                        OrderTypeId = orderTypeId,
                        CategoryId = categoryId,
                        SupplierId = supplierId,
                        ImagePath = item.Image,
                        IsActive = item.Active,
                        AllowDecimal = item.AllowDecimal,
                        DefaultUnitId = unitId,
                        OutOfStock = false
                    },
                    transaction);

                // Insert Product Unit
                await connection.ExecuteAsync(
                    """
                    INSERT INTO ProductUnits
                    (
                        ProductId,
                        UnitId,
                        IsDefault,
                        DisplayOrder
                    )
                    VALUES
                    (
                        @ProductId,
                        @UnitId,
                        1,
                        1
                    )
                    """,
                    new
                    {
                        ProductId = productId,
                        UnitId = unitId
                    },
                    transaction);

                imported++;
            }

            transaction.Commit();

            Console.WriteLine($"Imported: {imported}");
            Console.WriteLine($"Skipped : {skipped}");
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}