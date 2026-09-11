using AlisPo.Api.Data;
using AlisPo.Api.DTOs.Orders;
using AlisPo.Api.Repositories.Interfaces;
using Dapper;

namespace AlisPo.Api.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    private sealed class ProductLookup
    {
        public int ProductId { get; init; }

        public string ProductCode { get; init; } = "";

        public string ProductName { get; init; } = "";

        public string ThaiName { get; init; } = "";

        public int DefaultUnitId { get; init; }

        public string UnitName { get; init; } = "";
    }



    private sealed class OrderRow
    {
        public int PurchaseOrderId { get; init; }
        public string PONumber { get; init; } = "";

        public int BranchId { get; init; }
        public string BranchCode { get; init; } = "";
        public string BranchName { get; init; } = "";

        public DateTime OrderDate { get; init; }

        public string Status { get; init; } = "";

        public DateTime CreatedAt { get; init; }

        public int PurchaseOrderDetailId { get; init; }

        public string ProductCode { get; init; } = "";

        public string ProductName { get; init; } = "";

        public string ThaiName { get; init; } = "";

        // เพิ่ม
        public string CategoryName { get; init; } = "";

        public string OrderTypeName { get; init; } = "";

        public string UnitName { get; init; } = "";

        public decimal Quantity { get; init; }

        public string? Remark { get; init; }
    }

    public OrderRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory
            ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<int> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var transaction =
            await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            var branchCode = await connection.ExecuteScalarAsync<string>(
                new CommandDefinition(
                    commandText: @"
SELECT CASE BranchID
    WHEN 8 THEN 'SM'
    WHEN 3 THEN 'NH'
    WHEN 4 THEN 'KT'
    WHEN 9 THEN 'HL'
    WHEN 2 THEN 'BT'
    WHEN 7 THEN 'HO'
    WHEN 11 THEN 'CA'
    ELSE ''
END
FROM Branches
WHERE BranchID = @BranchId",
                    parameters: new
                    {
                        request.BranchId
                    },
                    transaction: transaction,
                    cancellationToken: cancellationToken,
                    commandTimeout: 30));

            var today = request.OrderDate.ToString("yyyyMMdd");

            var runningNo = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    commandText: @"
SELECT ISNULL(MAX(CAST(RIGHT(PONumber,4) AS INT)),0) + 1
FROM PurchaseOrders
WHERE BranchId = @BranchId
AND YEAR(OrderDate) = YEAR(@OrderDate)
AND MONTH(OrderDate) = MONTH(@OrderDate);",
                    parameters: new
                    {
                        request.BranchId,
                        request.OrderDate
                    },
                    transaction: transaction,
                    cancellationToken: cancellationToken,
                    commandTimeout: 30));

            Console.WriteLine($"BranchId={request.BranchId}");
            Console.WriteLine($"OrderDate={request.OrderDate:yyyy-MM-dd}");
            Console.WriteLine($"RunningNo={runningNo}");

            var poNumber =
    $"PO-{branchCode}-{today}-{runningNo:0000}";

            Console.WriteLine($"PONumber={poNumber}");

            var orderId = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    commandText: @"
INSERT INTO PurchaseOrders
(
    PONumber,
    BranchId,
    OrderDate,
    Status,
    CreatedBy,
    CreatedAt
)
OUTPUT INSERTED.PurchaseOrderId
VALUES
(
    @PONumber,
    @BranchId,
    @OrderDate,
    'Pending',
    @CreatedBy,
    GETDATE()
);",
                    parameters: new
                    {
                        PONumber = poNumber,
                        request.BranchId,
                        request.OrderDate,
                        request.CreatedBy
                    },
                    transaction: transaction,
                    cancellationToken: cancellationToken,
                    commandTimeout: 30));

            // Load every product used by this order in one query.
            // This avoids one SELECT query per line item.
            var productCodes = request.Items
                .Select(item => item.ProductCode?.Trim() ?? string.Empty)
                .Where(code => code.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var products = await connection.QueryAsync<ProductLookup>(
                new CommandDefinition(
                    commandText: @"
SELECT
    p.ProductId,
    p.ProductCode,
    p.ProductName,
    ISNULL(p.ThaiName,'') AS ThaiName,
    p.DefaultUnitId,
    u.UnitName
FROM Products p
INNER JOIN Units u
    ON p.DefaultUnitId = u.UnitId
WHERE p.ProductCode IN @ProductCodes;",
                    parameters: new
                    {
                        ProductCodes = productCodes
                    },
                    transaction: transaction,
                    cancellationToken: cancellationToken,
                    commandTimeout: 30));

            var productByCode = products.ToDictionary(
                product => product.ProductCode,
                StringComparer.OrdinalIgnoreCase);

            // Validate the complete order before writing any detail rows.
            foreach (var item in request.Items)
            {
                var productCode = item.ProductCode?.Trim() ?? string.Empty;

                if (productCode.Length == 0 ||
                    !productByCode.ContainsKey(productCode))
                {
                    throw new InvalidOperationException(
                        $"ProductCode '{productCode}' was not found.");
                }
            }

            // Keep SQL Server parameter usage comfortably below its 2100-parameter limit.
            // Each detail row uses 10 parameters, so 100 rows = 1000 parameters.
            const int batchSize = 100;

            for (var offset = 0; offset < request.Items.Count; offset += batchSize)
            {
                var batch = request.Items
                    .Skip(offset)
                    .Take(batchSize)
                    .ToList();

                var values = new List<string>(batch.Count);
                var parameters = new DynamicParameters();

                for (var index = 0; index < batch.Count; index++)
                {
                    var item = batch[index];
                    var product = productByCode[item.ProductCode.Trim()];
                    var prefix = $"p{index}_";

                    values.Add($"(@{prefix}PurchaseOrderId, @{prefix}ProductId, @{prefix}ProductCode, @{prefix}ProductName, @{prefix}ThaiName, @{prefix}UnitId, @{prefix}UnitName, @{prefix}Quantity, @{prefix}Remark, @{prefix}DisplayOrder)");

                    parameters.Add($"{prefix}PurchaseOrderId", orderId);
                    parameters.Add($"{prefix}ProductId", product.ProductId);
                    parameters.Add($"{prefix}ProductCode", product.ProductCode);
                    parameters.Add($"{prefix}ProductName", product.ProductName);
                    parameters.Add($"{prefix}ThaiName", product.ThaiName);
                    parameters.Add($"{prefix}UnitId", item.UnitId);
                    parameters.Add($"{prefix}UnitName", product.UnitName);
                    parameters.Add($"{prefix}Quantity", item.Qty);
                    parameters.Add($"{prefix}Remark", item.Remark ?? string.Empty);
                    parameters.Add($"{prefix}DisplayOrder", 0);
                }

                var insertSql = $@"
INSERT INTO PurchaseOrderDetails
(
    PurchaseOrderId,
    ProductId,
    ProductCode,
    ProductName,
    ThaiName,
    UnitId,
    UnitName,
    Quantity,
    Remark,
    DisplayOrder
)
VALUES
{string.Join(",\n", values)};";

                await connection.ExecuteAsync(
                    new CommandDefinition(
                        insertSql,
                        parameters,
                        transaction: transaction,
                        cancellationToken: cancellationToken,
                        commandTimeout: 30));
            }

            await transaction.CommitAsync(cancellationToken);

            return orderId;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<List<OrderHistoryDto>> GetOrdersAsync(
    CancellationToken cancellationToken = default)
    {


        using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        var sql = @"
SELECT

    po.PurchaseOrderId,
    po.PONumber,

    po.BranchId,
    CASE b.BranchID
        WHEN 8 THEN 'SM'
        WHEN 3 THEN 'NH'
        WHEN 4 THEN 'KT'
        WHEN 9 THEN 'HL'
        WHEN 2 THEN 'BT'
        WHEN 7 THEN 'HO'
        WHEN 10 THEN 'TECH'
        ELSE ''
    END AS BranchCode,
    b.BranchName,

    po.OrderDate,
    po.Status,
    po.CreatedAt,

    pod.PurchaseOrderDetailId,

    pod.ProductCode,
    pod.ProductName,
    ISNULL(pod.ThaiName,'') AS ThaiName,

    c.CategoryName,

    ot.OrderTypeName,

    pod.UnitName,

    pod.Quantity,

    ISNULL(pod.Remark,'') AS Remark

FROM PurchaseOrders po

INNER JOIN Branches b
ON po.BranchId=b.BranchId

INNER JOIN PurchaseOrderDetails pod
ON po.PurchaseOrderId=pod.PurchaseOrderId

INNER JOIN Products p
ON pod.ProductId=p.ProductId

INNER JOIN Categories c
ON p.CategoryId=c.CategoryId

INNER JOIN OrderTypes ot
ON c.OrderTypeId=ot.OrderTypeId

INNER JOIN Units u
ON pod.UnitId=u.UnitId

ORDER BY
po.CreatedAt DESC,
pod.PurchaseOrderDetailId ASC;";

        var rows = (await connection.QueryAsync<OrderRow>(
            new CommandDefinition(
                sql,
                cancellationToken: cancellationToken)))
            .ToList();

        var result = rows
            .GroupBy(x => x.PurchaseOrderId)
            .Select(g => new OrderHistoryDto
            {
                PurchaseOrderId = g.Key,
                PONumber = g.First().PONumber,
                BranchId = g.First().BranchId,
                BranchCode = g.First().BranchCode,
                BranchName = g.First().BranchName,
                OrderDate = g.First().OrderDate,
                Status = g.First().Status,
                CreatedAt = g.First().CreatedAt,

                Items = g.Select(i => new OrderHistoryItemDto
                {
                    PurchaseOrderDetailId = i.PurchaseOrderDetailId,

                    ProductCode = i.ProductCode,

                    ProductName = i.ProductName,

                    ThaiName = i.ThaiName,

                    CategoryName = i.CategoryName,

                    OrderTypeName = i.OrderTypeName,

                    UnitName = i.UnitName,

                    Quantity = i.Quantity,

                    Remark = i.Remark ?? ""
                }).ToList()
            })
            .ToList();

        return result;
    }

    public async Task DeleteOrderAsync(
    int purchaseOrderId,
    CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var transaction =
            await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await connection.ExecuteAsync(
                new CommandDefinition(
                    @"
DELETE FROM PurchaseOrderDetails
WHERE PurchaseOrderId = @PurchaseOrderId;",
                    new
                    {
                        PurchaseOrderId = purchaseOrderId
                    },
                    transaction,
                    cancellationToken: cancellationToken));

            await connection.ExecuteAsync(
                new CommandDefinition(
                    @"
DELETE FROM PurchaseOrders
WHERE PurchaseOrderId = @PurchaseOrderId;",
                    new
                    {
                        PurchaseOrderId = purchaseOrderId
                    },
                    transaction,
                    cancellationToken: cancellationToken));

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<int> DeleteOrdersInMonthAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var transaction =
            await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            var deletedCount = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    @"
SELECT COUNT(*)
FROM PurchaseOrders
WHERE YEAR(OrderDate) = @Year
AND MONTH(OrderDate) = @Month;",
                    new
                    {
                        Year = year,
                        Month = month
                    },
                    transaction,
                    cancellationToken: cancellationToken));

            await connection.ExecuteAsync(
                new CommandDefinition(
                    @"
DELETE pod
FROM PurchaseOrderDetails pod
INNER JOIN PurchaseOrders po
ON pod.PurchaseOrderId = po.PurchaseOrderId
WHERE YEAR(po.OrderDate) = @Year
AND MONTH(po.OrderDate) = @Month;",
                    new
                    {
                        Year = year,
                        Month = month
                    },
                    transaction,
                    cancellationToken: cancellationToken));

            await connection.ExecuteAsync(
                new CommandDefinition(
                    @"
DELETE FROM PurchaseOrders
WHERE YEAR(OrderDate) = @Year
AND MONTH(OrderDate) = @Month;",
                    new
                    {
                        Year = year,
                        Month = month
                    },
                    transaction,
                    cancellationToken: cancellationToken));

            await transaction.CommitAsync(cancellationToken);

            return deletedCount;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    public async Task CompleteOrderAsync(
        int purchaseOrderId,
        CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(
                @"
UPDATE PurchaseOrders
SET Status = 'Completed'
WHERE PurchaseOrderId = @PurchaseOrderId;",
                new
                {
                    PurchaseOrderId = purchaseOrderId
                },
                cancellationToken: cancellationToken));

        if (affectedRows == 0)
        {
            throw new KeyNotFoundException(
                $"PurchaseOrderId {purchaseOrderId} was not found.");
        }
    }
}
