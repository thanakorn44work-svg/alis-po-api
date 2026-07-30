namespace AlisPo.Api.Models;

public class Product
{
    public int ProductId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public int OrderTypeId { get; set; }

    public int? SupplierId { get; set; }

    public string? ImagePath { get; set; }

    public bool IsActive { get; set; }
}