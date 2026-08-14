namespace AlisPo.Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<UserInfo?> GetByBranchIdAsync(
        int branchId,
        CancellationToken cancellationToken = default);
}

public sealed class UserInfo
{
    public int UserId { get; init; }
    public string Username { get; init; } = "";
    public int BranchId { get; init; }
}