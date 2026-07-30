using AlisPo.Api.DTOs.Orders;
using AlisPo.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AlisPo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService
            ?? throw new ArgumentNullException(nameof(orderService));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var orderId = await _orderService.CreateOrderAsync(
            request,
            cancellationToken);

        return Ok(new
        {
            Success = true,
            PurchaseOrderId = orderId
        });
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders(
        CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetOrdersAsync(
            cancellationToken);

        return Ok(orders);
    }

    [HttpDelete("{purchaseOrderId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrder(
        int purchaseOrderId,
        CancellationToken cancellationToken)
    {
        await _orderService.DeleteOrderAsync(
            purchaseOrderId,
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("month/{year:int}/{month:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteOrdersInMonth(
        int year,
        int month,
        CancellationToken cancellationToken)
    {
        var deletedCount = await _orderService.DeleteOrdersInMonthAsync(
            year,
            month,
            cancellationToken);

        return Ok(new
        {
            Success = true,
            DeletedCount = deletedCount
        });
    }

}