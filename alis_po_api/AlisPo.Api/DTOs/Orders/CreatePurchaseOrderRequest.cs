namespace AlisPo.Api.DTOs.Orders;

public class CreatePurchaseOrderRequest
{
    public int BranchId { get; set; }

    public int CreatedBy { get; set; }

    public List<CreatePurchaseOrderItemRequest> Items { get; set; } = new();

    public List<CreatePurchaseOrderRemarkRequest> Remarks { get; set; } = new();
}