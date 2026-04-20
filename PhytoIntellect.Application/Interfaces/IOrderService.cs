using PhytoIntellect.Application.Contracts.Orders;

namespace PhytoIntellect.Application.Interfaces;

public interface IOrderService
{
    Task<string> CreateOrderAsync(string userId, CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderSummaryResponse>> GetPatientOrdersAsync(string userId, CancellationToken cancellationToken = default);
    Task<OrderDetailsResponse> GetOrderDetailsForPatientAsync(int orderId, string userId, CancellationToken cancellationToken = default);
    Task CancelOrderAsync(int orderId, string userId, CancellationToken cancellationToken = default);
    Task<string> SimulatePaymentAsync(int orderId, string userId, CancellationToken cancellationToken = default);

    Task<bool> ToggleFavoriteOrderAsync(string userId, int orderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderSummaryResponse>> GetFavoriteOrdersAsync(string userId, CancellationToken cancellationToken = default);
}