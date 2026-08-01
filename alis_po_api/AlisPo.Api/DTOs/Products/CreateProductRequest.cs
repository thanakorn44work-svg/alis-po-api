namespace AlisPo.Api.DTOs.Products;

public class CreateProductRequest
{
    public string ProductCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string ThaiName { get; set; } = string.Empty;

    public string MainCategory { get; set; } = string.Empty;

    public string SubCategory { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public bool AllowDecimal { get; set; }

    public bool Active { get; set; }

    public bool OutOfStock { get; set; }
}