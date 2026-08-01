namespace AlisPo.Api.DTOs.Orders;

public class CreatePurchaseOrderRemarkRequest
{
    public int OrderTypeId { get; set; }

    public string? Remark { get; set; }
}