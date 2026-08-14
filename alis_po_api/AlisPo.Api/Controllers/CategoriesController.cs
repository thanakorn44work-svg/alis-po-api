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

    [HttpPost("sub")]
    public async Task<ActionResult> AddSubCategory(
    [FromQuery] string mainCategory,
    [FromQuery] string subCategory,
    CancellationToken cancellationToken)
    {
        await _service.AddSubCategoryAsync(
            mainCategory,
            subCategory,
            cancellationToken);

        return NoContent();
    }
    [HttpPost("main")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddMainCategory(
    [FromQuery] string mainCategory,
    CancellationToken cancellationToken)
    {
        await _service.AddMainCategoryAsync(
            mainCategory,
            cancellationToken);

        return NoContent();
    }
    [HttpDelete("sub")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteSubCategory(
    [FromQuery] string mainCategory,
    [FromQuery] string subCategory,
    CancellationToken cancellationToken)
    {
        await _service.DeleteSubCategoryAsync(
            mainCategory,
            subCategory,
            cancellationToken);

        return NoContent();
    }
    [HttpDelete("main")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteMainCategory(
    [FromQuery] string mainCategory,
    CancellationToken cancellationToken)
    {
        await _service.DeleteMainCategoryAsync(
            mainCategory,
            cancellationToken);

        return NoContent();
    }
    [HttpPut("sub")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RenameSubCategory(
    [FromQuery] string mainCategory,
    [FromQuery] string oldName,
    [FromQuery] string newName,
    CancellationToken cancellationToken)
    {
        await _service.RenameSubCategoryAsync(
            mainCategory,
            oldName,
            newName,
            cancellationToken);

        return NoContent();
    }
    [HttpPut("main")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RenameMainCategory(
    [FromQuery] string oldName,
    [FromQuery] string newName,
    CancellationToken cancellationToken)
    {
        await _service.RenameMainCategoryAsync(
            oldName,
            newName,
            cancellationToken);

        return NoContent();
    }
}