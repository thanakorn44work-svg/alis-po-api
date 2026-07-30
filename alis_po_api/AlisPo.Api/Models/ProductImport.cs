namespace AlisPo.Api.Models;

public class ProductImport
{
    public string Id { get; set; } = "";

    public string MainCategory { get; set; } = "";

    public string SubCategory { get; set; } = "";

    public string Name { get; set; } = "";

    public string ThaiName { get; set; } = "";

    public string Image { get; set; } = "";

    public string Unit { get; set; } = "";

    public bool AllowDecimal { get; set; }

    public bool Active { get; set; }
}