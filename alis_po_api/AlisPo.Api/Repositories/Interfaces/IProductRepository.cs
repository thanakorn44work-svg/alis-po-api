using AlisPo.Api.DTOs;
using AlisPo.Api.DTOs.Products;

namespace AlisPo.Api.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<ProductDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default);
}