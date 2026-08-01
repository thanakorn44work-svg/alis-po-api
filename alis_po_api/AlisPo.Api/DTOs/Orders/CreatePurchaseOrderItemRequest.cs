namespace AlisPo.Api.DTOs.Orders;

public class CreatePurchaseOrderItemRequest
{
    public int ProductId { get; set; }

    public int UnitId { get; set; }

    public decimal Qty { get; set; }

    public int DisplayOrder { get; set; }
}