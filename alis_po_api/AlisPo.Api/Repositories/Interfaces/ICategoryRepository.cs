using AlisPo.Api.DTOs;

namespace AlisPo.Api.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task AddSubCategoryAsync(
        string mainCategory,
        string subCategory,
        CancellationToken cancellationToken = default);

    Task AddMainCategoryAsync(
    string mainCategory,
    CancellationToken cancellationToken = default);

    Task DeleteSubCategoryAsync(
    string mainCategory,
    string subCategory,
    CancellationToken cancellationToken = default);

    Task DeleteMainCategoryAsync(
    string mainCategory,
    CancellationToken cancellationToken = default);

    Task RenameSubCategoryAsync(
    string mainCategory,
    string oldName,
    string newName,
    CancellationToken cancellationToken = default);

    Task RenameMainCategoryAsync(
    string oldName,
    string newName,
    CancellationToken cancellationToken = default);
}