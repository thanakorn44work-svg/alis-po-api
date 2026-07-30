using AlisPo.Api.DTOs.Products;
using AlisPo.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AlisPo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service
            ?? throw new ArgumentNullException(nameof(service));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Get(
        CancellationToken cancellationToken)
    {
        var products = await _service.GetProductsAsync(cancellationToken);

        return Ok(products);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Post(
    [FromBody] CreateProductRequest request,
    CancellationToken cancellationToken)
    {
        await _service.AddProductAsync(
            request,
            cancellationToken);

        return Created();
    }
}