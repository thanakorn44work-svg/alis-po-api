namespace AlisPo.Api.DTOs.Orders;

public sealed class CreateOrderItemRequest
{
    public string ProductCode { get; set; } = string.Empty;

    public decimal Qty { get; set; }

    public string? Remark { get; set; }

    public int UnitId { get; set; }
}