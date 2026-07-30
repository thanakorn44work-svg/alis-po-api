namespace AlisPo.Api.DTOs.Orders;

public sealed class OrderHistoryItemDto
{
    public int PurchaseOrderDetailId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string ThaiName { get; set; } = string.Empty;

    // Main Supplier
    public string OrderTypeName { get; set; } = string.Empty;

    // Chicken / Sauce / Vegetable ...
    public string CategoryName { get; set; } = string.Empty;

    // KG / PCS / BOX
    public string UnitName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public string Remark { get; set; } = string.Empty;
}