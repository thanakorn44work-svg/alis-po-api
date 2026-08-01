namespace AlisPo.Api.DTOs;

public sealed class CategoryDto
{
    public string MainCategory { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public int MainDisplayOrder { get; set; }
    public int SubDisplayOrder { get; set; }
}