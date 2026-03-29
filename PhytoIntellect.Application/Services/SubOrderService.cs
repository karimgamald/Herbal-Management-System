using AutoMapper;
using PhytoIntellect.Application.Contracts.Orders;
using PhytoIntellect.Application.Contracts.SubOrders;
using PhytoIntellect.Application.Interfaces;

public class SubOrderService(IUnitOfWork unitOfWork, IMapper mapper) : ISubOrderService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    // =============================
    // ✅ 1. Get Herbalist SubOrders
    // =============================
    public async Task<IEnumerable<SubOrderSummaryResponse>> GetHerbalistSubOrdersAsync(
        string userId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId))
            return Enumerable.Empty<SubOrderSummaryResponse>();

        var herbalist = await _unitOfWork.HerbalistRepository.GetAsync(
            h => h.UserId == parsedUserId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            return Enumerable.Empty<SubOrderSummaryResponse>();

        var subOrders = await _unitOfWork.SubOrderRepository.GetAllAsync(
            s => s.HerbalistId == herbalist.HerbalistId,
            includeProperties: "Herbalist.User",
            tracked: false,
            cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<SubOrderSummaryResponse>>(subOrders);
    }

    // =============================
    // ✅ 2. Get SubOrder Details
    // =============================
    public async Task<SubOrderDetailsResponse?> GetSubOrderDetailsAsync(
        int subOrderId, string userId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId))
            return null;

        var herbalist = await _unitOfWork.HerbalistRepository.GetAsync(
            h => h.UserId == parsedUserId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            return null;

        var subOrder = await _unitOfWork.SubOrderRepository.GetAsync(
            s => s.SubOrderId == subOrderId && s.HerbalistId == herbalist.HerbalistId,
            includeProperties: "OrderRecipes.Recipe,OrderHerbs.Herb",
            tracked: false,
            cancellationToken: cancellationToken);

        if (subOrder == null)
            return null;

        return new SubOrderDetailsResponse
        {
            SubOrderId = subOrder.SubOrderId,
            SubTotal = subOrder.SubTotal,
            Status = subOrder.Status,
            TrackingNumber = subOrder.TrackingNumber,

            Recipes = subOrder.OrderRecipes?
                .Select(r => new OrderItemResponse
                {
                    ItemId = r.RecipeId,
                    Name = r.Recipe?.Description ?? "Recipe",
                    Quantity = r.Quantity,
                    UnitPrice = r.UnitPrice,
                    SubTotal = r.SubTotal
                }).ToList() ?? new List<OrderItemResponse>(),

            Herbs = subOrder.OrderHerbs?
                .Select(h => new OrderItemResponse
                {
                    ItemId = h.HerbId,
                    Name = h.Herb?.HerbName ?? "Herb",
                    Quantity = h.Quantity,
                    UnitPrice = h.UnitPrice,
                    SubTotal = h.SubTotal
                }).ToList() ?? new List<OrderItemResponse>()
        };
    }

    // =============================
    // ✅ 3. Update Status
    // =============================
    public async Task UpdateSubOrderStatusAsync(
        int subOrderId,
        string userId,
        UpdateSubOrderStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId))
            throw new Exception("Invalid User ID.");

        var herbalist = await _unitOfWork.HerbalistRepository.GetAsync(
            h => h.UserId == parsedUserId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            throw new Exception("Herbalist not found.");

        var subOrder = await _unitOfWork.SubOrderRepository.GetAsync(
            s => s.SubOrderId == subOrderId && s.HerbalistId == herbalist.HerbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (subOrder == null)
            throw new Exception("SubOrder not found or access denied.");

        // =============================
        // ✅ Validate Status
        // =============================
        var allowedStatuses = new[] { "Pending", "Accepted", "Shipped", "Delivered", "Cancelled" };

        if (!allowedStatuses.Contains(request.Status))
            throw new InvalidOperationException("Invalid status value.");

        // =============================
        // ✅ Update Status
        // =============================
        subOrder.Status = request.Status;

        // =============================
        // ✅ Tracking Number Logic
        // =============================
        if (!string.IsNullOrWhiteSpace(request.TrackingNumber))
        {
            subOrder.TrackingNumber = request.TrackingNumber;
        }
        else if (request.Status == "Shipped" && string.IsNullOrWhiteSpace(subOrder.TrackingNumber))
        {
            subOrder.TrackingNumber = GenerateTrackingNumber(herbalist.HerbalistId);
        }

        _unitOfWork.SubOrderRepository.Update(subOrder);

        // =============================
        // ✅ Update Main Order Status
        // =============================
        if (request.Status == "Delivered")
        {
            var mainOrder = await _unitOfWork.OrderRepository.GetAsync(
                o => o.OrderId == subOrder.OrderId,
                includeProperties: "SubOrders",
                tracked: true,
                cancellationToken: cancellationToken);

            if (mainOrder != null && mainOrder.SubOrders.All(s => s.Status == "Delivered"))
            {
                mainOrder.OrderStatus = "Completed";
                _unitOfWork.OrderRepository.Update(mainOrder);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    // =============================
    // ✅ Tracking Generator
    // =============================
    private string GenerateTrackingNumber(int herbalistId)
    {
        return $"PHYTO-H{herbalistId}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
    }
}