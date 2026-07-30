using AlisPo.Api.DTOs.Orders;
using AlisPo.Api.Repositories.Interfaces;
using AlisPo.Api.Services.Interfaces;

namespace AlisPo.Api.Services;

/// <summary>
/// Business logic สำหรับใบสั่งซื้อ
/// </summary>
public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<int> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await _repository.CreateOrderAsync(
            request,
            cancellationToken);
    }

    public async Task<List<OrderHistoryDto>> GetOrdersAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetOrdersAsync(
            cancellationToken);
    }

    public async Task DeleteOrderAsync(
        int purchaseOrderId,
        CancellationToken cancellationToken = default)
    {
        await _repository.DeleteOrderAsync(
            purchaseOrderId,
            cancellationToken);
    }

    public async Task<int> DeleteOrdersInMonthAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        return await _repository.DeleteOrdersInMonthAsync(
            year,
            month,
            cancellationToken);
    }
}