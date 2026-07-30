using AlisPo.Api.DTOs;
using AlisPo.Api.Repositories.Interfaces;
using AlisPo.Api.Services.Interfaces;

namespace AlisPo.Api.Services;

/// <summary>
/// Service สำหรับจัดการหมวดหมู่สินค้า
/// </summary>
public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}