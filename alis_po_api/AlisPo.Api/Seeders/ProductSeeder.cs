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

    private static readonly IReadOnlyDictionary<string, string> SupplierMap =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Makro Order"] = "MAKRO",
            ["Store Order"] = "STORE",
            ["Kanna Juice Order"] = "KANNA",
            ["Fruit Order"] = "FRUIT",
            ["Bread Order"] = "BREAD",
            ["Cake Order"] = "CAKE",
            ["Supercheap Order"] = "SUPERCHEAP"
        };

    public ProductSeeder(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task SeedAsync()
    {
        var file = Path.Combine(AppContext.BaseDirectory, "Imports", "products.json");

        if (!File.Exists(file))
            throw new FileNotFoundException("products.json not found.", file);

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
                    "SELECT COUNT(*) FROM Products WHERE ProductCode=@Code",
                    new { Code = item.Id }, transaction);

                if (exists > 0)
                {
                    skipped++;
                    continue;
                }

                var orderTypeId = await connection.ExecuteScalarAsync<int?>(
                    "SELECT OrderTypeId FROM OrderTypes WHERE OrderTypeName=@Name",
                    new { Name = item.MainCategory }, transaction)
                    ?? throw new Exception($"OrderType not found: {item.MainCategory}");

                var categoryId = await connection.ExecuteScalarAsync<int?>(
                    @"SELECT CategoryId
                      FROM Categories
                      WHERE OrderTypeId=@OrderTypeId
                      AND CategoryName=@CategoryName",
                    new { OrderTypeId = orderTypeId, CategoryName = item.SubCategory }, transaction)
                    ?? throw new Exception($"Category not found: {item.SubCategory}");

                var unitId = await connection.ExecuteScalarAsync<int?>(
                    "SELECT UnitId FROM Units WHERE UnitCode=@Code",
                    new { Code = item.Unit }, transaction)
                    ?? throw new Exception($"Unit not found: {item.Unit}");

                if (!SupplierMap.TryGetValue(item.MainCategory, out var supplierCode))
                    throw new Exception($"Unknown MainCategory: {item.MainCategory}");

                var supplierId = await connection.ExecuteScalarAsync<int?>(
                    "SELECT SupplierId FROM Suppliers WHERE SupplierCode=@Code",
                    new { Code = supplierCode }, transaction)
                    ?? throw new Exception($"Supplier not found: {supplierCode}");

                var productId = await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO Products
                (ProductCode,ProductName,ThaiName,OrderTypeId,CategoryId,SupplierId,
                 ImagePath,Description,DisplayOrder,IsActive,CreatedAt,UpdatedAt,
                 AllowDecimal,DefaultUnitId)
                 OUTPUT INSERTED.ProductId
                 VALUES
                (@ProductCode,@ProductName,@ThaiName,@OrderTypeId,@CategoryId,@SupplierId,
                 @ImagePath,'',0,@IsActive,GETDATE(),GETDATE(),@AllowDecimal,@DefaultUnitId)",
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
                    DefaultUnitId = unitId
                }, transaction);

                await connection.ExecuteAsync(
                @"INSERT INTO ProductUnits
                  (ProductId,UnitId,IsDefault,DisplayOrder)
                  VALUES(@ProductId,@UnitId,1,1)",
                new { ProductId = productId, UnitId = unitId }, transaction);

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
