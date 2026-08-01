namespace AlisPo.Api.DTOs.Orders;

public class CreatePurchaseOrderResponse
{
    public int PurchaseOrderId { get; set; }

    public string PONumber { get; set; } = string.Empty;

    public DateTime SubmittedAt { get; set; }
}