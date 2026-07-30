using AlisPo.Api.DTOs;
using AlisPo.Api.DTOs.Products;

namespace AlisPo.Api.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync(
        CancellationToken cancellationToken = default);

    Task AddProductAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default);
}