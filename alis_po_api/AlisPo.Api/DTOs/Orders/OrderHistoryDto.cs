namespace AlisPo.Api.DTOs.Orders;

public sealed class OrderHistoryDto
{
    public int PurchaseOrderId { get; set; }

    public string PONumber { get; set; } = string.Empty;

    public int BranchId { get; set; }

    public string BranchCode { get; set; } = string.Empty;

    public string BranchName { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public List<OrderHistoryItemDto> Items { get; set; }
        = new();
}