using AlisPo.Api.DTOs;

namespace AlisPo.Api.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryDto>> GetAllAsync(
        CancellationToken cancellationToken = default);
}