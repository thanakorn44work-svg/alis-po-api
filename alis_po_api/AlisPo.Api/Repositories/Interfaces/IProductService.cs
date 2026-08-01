using AlisPo.Api.DTOs;
using AlisPo.Api.DTOs.Products;

namespace AlisPo.Api.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync(
        CancellationToken cancellationToken = default);

    Task<ProductDto?> GetProductByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task AddProductAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default);

    Task UpdateProductAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteProductAsync(
        int id,
        CancellationToken cancellationToken = default);
}