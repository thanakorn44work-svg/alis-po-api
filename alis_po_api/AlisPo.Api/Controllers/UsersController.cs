using AlisPo.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AlisPo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository
            ?? throw new ArgumentNullException(nameof(userRepository));
    }

    [HttpGet("branch/{branchId:int}")]
    public async Task<IActionResult> GetByBranchId(
        int branchId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByBranchIdAsync(
            branchId,
            cancellationToken);

        if (user is null)
        {
            return NotFound(new
            {
                message = $"No active user found for BranchId {branchId}."
            });
        }

        return Ok(user);
    }
}