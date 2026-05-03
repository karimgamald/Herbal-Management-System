using PhytoIntellect.Application.Contracts.Financials;
using PhytoIntellect.Application.Contracts.SubOrders;
using PhytoIntellect.Application.Paginations;

namespace PhytoIntellect.Application.Interfaces;

public interface ISubOrderService
{
    Task<PaginatedList<SubOrderSummaryResponse>> GetHerbalistSubOrdersAsync(string userId, RequestFilters filters, CancellationToken cancellationToken = default);
    Task<SubOrderDetailsResponse> GetSubOrderDetailsAsync(int subOrderId, string userId, CancellationToken cancellationToken = default);
    Task UpdateSubOrderStatusAsync(int subOrderId, string userId, UpdateSubOrderStatusRequest request, CancellationToken cancellationToken = default);
    Task<HerbalistFinancialDashboardResponse> GetHerbalistFinancialsAsync(string userId, CancellationToken cancellationToken = default);
}