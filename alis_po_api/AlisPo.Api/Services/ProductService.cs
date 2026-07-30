using AlisPo.Api.DTOs;
using AlisPo.Api.Repositories;
using AlisPo.Api.Services.Interfaces;
using AlisPo.Api.DTOs.Products;

namespace AlisPo.Api.Services;

/// <summary>
/// Business logic สำหรับสินค้า
/// </summary>
public sealed class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
    public async Task AddProductAsync(
    CreateProductRequest request,
    CancellationToken cancellationToken = default)
    {
        await _repository.AddAsync(
            request,
            cancellationToken);
    }
}