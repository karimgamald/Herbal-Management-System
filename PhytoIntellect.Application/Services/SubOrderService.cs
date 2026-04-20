using AutoMapper;
using PhytoIntellect.Application.Contracts.Orders;
using PhytoIntellect.Application.Contracts.SubOrders;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Enums;

namespace PhytoIntellect.Application.Services;

public class SubOrderService(IUnitOfWork unitOfWork, IMapper mapper) : ISubOrderService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<SubOrderSummaryResponse>> GetHerbalistSubOrdersAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId)) 
            return Enumerable.Empty<SubOrderSummaryResponse>();

        var herbalist = await _unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == parsedUserId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null) 
            return Enumerable.Empty<SubOrderSummaryResponse>();

        var subOrders = await _unitOfWork.SubOrderRepository.GetAllAsync(
            filter: s => s.HerbalistId == herbalist.HerbalistId,
            includeProperties: "Herbalist.User",
            tracked: false,
            cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<SubOrderSummaryResponse>>(subOrders);
    }

    public async Task<SubOrderDetailsResponse?> GetSubOrderDetailsAsync(int subOrderId, string userId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId)) 
            return null;

        var herbalist = await _unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == parsedUserId, tracked: false, cancellationToken: cancellationToken);

        if (herbalist == null) 
            return null;

        var subOrder = await _unitOfWork.SubOrderRepository.GetAsync(
            filter: s => s.SubOrderId == subOrderId && s.HerbalistId == herbalist.HerbalistId,
            includeProperties: "OrderRecipes.Recipe,OrderHerbs.Herb,OrderAiRecipes.AiRecipe",
            tracked: false,
            cancellationToken: cancellationToken);

        if (subOrder == null) 
            return null;

        return new SubOrderDetailsResponse 
        {
            SubOrderId = subOrder.SubOrderId,
            SubTotal = subOrder.SubTotal,
            Status = subOrder.Status,
            TrackingNumber = subOrder.ExternalDeliveryID,

            Recipes = subOrder.OrderRecipes.Select(r => new OrderRecipeResponse
            {
                RecipeId = r.RecipeId,
                RecipeName = r.Recipe!.Description ?? "Recipe",
                QuantityPerOne = r.Quantity,
                UnitPricePerOne = r.UnitPrice,
                SubTotal = r.SubTotal,
            }).ToList(),

            Herbs = subOrder.OrderHerbs.Select(h => new OrderHerbResponse
            {
                HerbId = h.HerbId,
                HerbName = h.Herb!.HerbName ?? "Herb", 
                QuantityPerGram = h.Quantity,
                UnitPricePerKilo = h.UnitPrice,
                SubTotal = h.SubTotal
            }).ToList(),

            AiRecipes = subOrder.OrderAiRecipes.Select(a => new OrderAiRecipeResponse
            {
                AiRecipeId = a.AiRecipeId,
                RecipeName = a.AiRecipe!.RecommendedRecipeName ?? "AI Recipe",
                Quantity = a.Quantity,
                UnitPrice = a.UnitPrice,
                SubTotal = a.SubTotal
            }).ToList()
        };
    }

    public async Task UpdateSubOrderStatusAsync(int subOrderId, string userId, UpdateSubOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId))
            throw new ArgumentException("Invalid User ID format.");

        var herbalist = await _unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == parsedUserId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null)
            throw new UnauthorizedAccessException("Herbalist account not found or access denied.");

        if (!Enum.TryParse<SubOrderStatus>(request.Status, true, out var newSubStatus))
            throw new ArgumentException("Invalid SubOrder Status. Please provide a valid status like Preparing or Shipped.");

        var subOrder = await _unitOfWork.SubOrderRepository.GetAsync(
            filter: s => s.SubOrderId == subOrderId && s.HerbalistId == herbalist.HerbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (subOrder == null)
            throw new KeyNotFoundException("SubOrder not found or you do not have permission to access it.");

        subOrder.Status = newSubStatus.ToString();

        if (newSubStatus == SubOrderStatus.Shipped && string.IsNullOrWhiteSpace(subOrder.ExternalDeliveryID))
        {
            subOrder.ExternalDeliveryID = GenerateTrackingNumber(herbalist.HerbalistId);
        }

        _unitOfWork.SubOrderRepository.Update(subOrder);

        var mainOrder = await _unitOfWork.OrderRepository.GetAsync(
            filter: o => o.OrderId == subOrder.OrderId,
            includeProperties: "SubOrders",
            tracked: true,
            cancellationToken: cancellationToken);

        if (mainOrder != null)
        {
            var allStatuses = mainOrder.SubOrders.Select(s => s.Status).ToList();
            mainOrder.OrderStatus = DetermineMainOrderStatus(allStatuses);
            _unitOfWork.OrderRepository.Update(mainOrder);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private string GenerateTrackingNumber(int herbalistId)
    {
        string randomString = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        return $"HERBALIST-H{herbalistId}-{randomString}";
    }

    private string DetermineMainOrderStatus(List<string> subStatuses)
    {
        if (subStatuses.All(s => s == SubOrderStatus.Cancelled.ToString()))
            return OrderStatus.Cancelled.ToString();

        if (subStatuses.All(s => s == SubOrderStatus.Delivered.ToString() || s == SubOrderStatus.Cancelled.ToString()))
        {
            if (subStatuses.Contains(SubOrderStatus.Cancelled.ToString()))
                return OrderStatus.PartiallyDelivered.ToString();

            return OrderStatus.Delivered.ToString();
        }

        if (subStatuses.All(s => s == SubOrderStatus.Shipped.ToString() || s == SubOrderStatus.Delivered.ToString() || s == SubOrderStatus.Cancelled.ToString()))
        {
            if (subStatuses.Contains(SubOrderStatus.Cancelled.ToString()))
                return OrderStatus.PartiallyShipped.ToString();

            return OrderStatus.Shipped.ToString();
        }

        if (subStatuses.Any(s => s == SubOrderStatus.Preparing.ToString() || s == SubOrderStatus.Shipped.ToString()))
            return OrderStatus.Processing.ToString();

        return OrderStatus.Pending.ToString();
    }
}