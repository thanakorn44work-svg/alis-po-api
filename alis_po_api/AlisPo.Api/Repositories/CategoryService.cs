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

    public async Task AddMainCategoryAsync(
    string mainCategory,
    CancellationToken cancellationToken = default)
    {
        await _repository.AddMainCategoryAsync(
            mainCategory,
            cancellationToken);
    }

    public async Task AddSubCategoryAsync(
    string mainCategory,
    string subCategory,
    CancellationToken cancellationToken = default)
    {
        await _repository.AddSubCategoryAsync(
            mainCategory,
            subCategory,
            cancellationToken);
    }
    public async Task DeleteSubCategoryAsync(
    string mainCategory,
    string subCategory,
    CancellationToken cancellationToken = default)
    {
        await _repository.DeleteSubCategoryAsync(
            mainCategory,
            subCategory,
            cancellationToken);
    }
    public async Task DeleteMainCategoryAsync(
    string mainCategory,
    CancellationToken cancellationToken = default)
    {
        await _repository.DeleteMainCategoryAsync(
            mainCategory,
            cancellationToken);
    }
    public async Task RenameSubCategoryAsync(
    string mainCategory,
    string oldName,
    string newName,
    CancellationToken cancellationToken = default)
    {
        await _repository.RenameSubCategoryAsync(
            mainCategory,
            oldName,
            newName,
            cancellationToken);
    }
    public async Task RenameMainCategoryAsync(
    string oldName,
    string newName,
    CancellationToken cancellationToken = default)
    {
        await _repository.RenameMainCategoryAsync(
            oldName,
            newName,
            cancellationToken);
    }
}