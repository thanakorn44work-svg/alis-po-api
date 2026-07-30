using AlisPo.Api.DTOs.Orders;

namespace AlisPo.Api.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<int> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<List<OrderHistoryDto>> GetOrdersAsync(
        CancellationToken cancellationToken = default);

    Task DeleteOrderAsync(
        int purchaseOrderId,
        CancellationToken cancellationToken = default);

    Task<int> DeleteOrdersInMonthAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);
}