namespace AlisPo.Api.DTOs.Orders;

public sealed class CreateOrderRequest
{
    public string PONumber { get; set; } = string.Empty;

    public int BranchId { get; set; }

    public int CreatedBy { get; set; }

    public DateTime OrderDate { get; set; }

    public string? Remark { get; set; }

    public List<CreateOrderItemRequest> Items { get; set; } = [];
}