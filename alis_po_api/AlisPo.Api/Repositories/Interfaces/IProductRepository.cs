using AlisPo.Api.DTOs;
using AlisPo.Api.DTOs.Products;

namespace AlisPo.Api.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<ProductDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ProductDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}