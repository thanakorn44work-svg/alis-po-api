using AlisPo.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AlisPo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service)
    {
        _service = service
            ?? throw new ArgumentNullException(nameof(service));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Get(
        CancellationToken cancellationToken)
    {
        var categories = await _service.GetCategoriesAsync(cancellationToken);

        return Ok(categories);
    }
}