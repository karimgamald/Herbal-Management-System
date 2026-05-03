using PhytoIntellect.Application.Contracts.Orders;
using PhytoIntellect.Application.Paginations;

namespace PhytoIntellect.Application.Interfaces;

public interface IOrderService
{
    Task<string> CreateOrderAsync(string userId, CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<PaginatedList<OrderSummaryResponse>> GetPatientOrdersAsync(string userId, RequestFilters filters, CancellationToken cancellationToken = default);
    Task<OrderDetailsResponse> GetOrderDetailsForPatientAsync(int orderId, string userId, CancellationToken cancellationToken = default);
    Task CancelOrderAsync(int orderId, string userId, CancellationToken cancellationToken = default);
    Task<string> SimulatePaymentAsync(int orderId, string userId, CancellationToken cancellationToken = default);

    Task<bool> ToggleFavoriteOrderAsync(string userId, int orderId, CancellationToken cancellationToken = default);
    Task<PaginatedList<OrderSummaryResponse>> GetFavoriteOrdersAsync(string userId, RequestFilters filters, CancellationToken cancellationToken = default);
}