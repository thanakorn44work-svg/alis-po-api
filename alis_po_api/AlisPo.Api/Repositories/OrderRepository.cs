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

        public int DefaultUnitId { get; init; }
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
SELECT BranchCode
FROM Branches
WHERE BranchId = @BranchId",
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
AND CAST(OrderDate AS date) = CAST(@OrderDate AS date);",
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

            request.PONumber = $"PO-{branchCode}-{today}-{runningNo:0000}";

            Console.WriteLine($"PONumber={request.PONumber}");

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
                        request.PONumber,
                        request.BranchId,
                        request.OrderDate,
                        request.CreatedBy
                    },
                    transaction: transaction,
                    cancellationToken: cancellationToken,
                    commandTimeout: 30));

            foreach (var item in request.Items)
            {
                var product = await connection.QuerySingleOrDefaultAsync<ProductLookup>(
                    new CommandDefinition(
                        commandText: @"
SELECT
    ProductId,
    DefaultUnitId
FROM Products
WHERE ProductCode = @ProductCode",
                        parameters: new
                        {
                            item.ProductCode
                        },
                        transaction: transaction,
                        cancellationToken: cancellationToken,
                        commandTimeout: 30));

                if (product is null)
                {
                    throw new InvalidOperationException(
                        $"ProductCode '{item.ProductCode}' was not found.");
                }

                await connection.ExecuteAsync(
                    new CommandDefinition(
                        commandText: @"
INSERT INTO PurchaseOrderDetails
(
    PurchaseOrderId,
    ProductId,
    UnitId,
    Quantity,
    Remark
)
VALUES
(
    @PurchaseOrderId,
    @ProductId,
    @UnitId,
    @Quantity,
    @Remark
);",
                        parameters: new
                        {
                            PurchaseOrderId = orderId,
                            ProductId = product.ProductId,
                            UnitId = item.UnitId,
                            Quantity = item.Qty,
                            item.Remark
                        },
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
    b.BranchCode,
    b.BranchName,

    po.OrderDate,
    po.Status,
    po.CreatedAt,

    pod.PurchaseOrderDetailId,

    p.ProductCode,
    p.ProductName,
    ISNULL(p.ThaiName,'') AS ThaiName,

    c.CategoryName,

    ot.OrderTypeName,

    u.UnitName,

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

}
