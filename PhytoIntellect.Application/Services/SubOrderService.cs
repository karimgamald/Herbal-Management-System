using AutoMapper;
using AutoMapper.QueryableExtensions;
using PhytoIntellect.Application.Contracts.Financials;
using PhytoIntellect.Application.Contracts.Orders;
using PhytoIntellect.Application.Contracts.SubOrders;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Enums;

namespace PhytoIntellect.Application.Services;

public class SubOrderService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService) : ISubOrderService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<PaginatedList<SubOrderSummaryResponse>> GetHerbalistSubOrdersAsync(
    string userId,
    RequestFilters filters,
    CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId))
            return new PaginatedList<SubOrderSummaryResponse>(
                new List<SubOrderSummaryResponse>(),
                0,
                filters.PageNumber,
                filters.PageSize);

        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            h => h.UserId == parsedUserId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            return new PaginatedList<SubOrderSummaryResponse>(
                new List<SubOrderSummaryResponse>(),
                0,
                filters.PageNumber,
                filters.PageSize);

        var query = _unitOfWork.SubOrderRepository
            .GetQueryable(tracked: false)
            .Where(s => s.HerbalistId == herbalist.HerbalistId);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();

            query = query.Where(s =>
                s.Herbalist!.User!.FullName.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = string.Equals(
                filters.SortDirection,
                "DESC",
                StringComparison.OrdinalIgnoreCase);

            query = filters.SortColumn.ToLower() switch
            {
                "id" => isDesc
                    ? query.OrderByDescending(s => s.SubOrderId)
                    : query.OrderBy(s => s.SubOrderId),


                "date" => isDesc
                    ? query.OrderByDescending(s => s.Order!.OrderDate)
                    : query.OrderBy(s => s.Order!.OrderDate),

                _ => isDesc
                    ? query.OrderByDescending(s => s.SubOrderId)
                    : query.OrderBy(s => s.SubOrderId)
            };
        }
        else
        {
            query = query.OrderByDescending(s => s.Order!.OrderDate);
        }

        var projectedQuery = query.ProjectTo<SubOrderSummaryResponse>(
            mapper.ConfigurationProvider);

        var paginatedResult = await PaginatedList<SubOrderSummaryResponse>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);

        return paginatedResult;
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
            includeProperties: "OrderRecipes.Recipe,OrderHerbs.Herb,OrderAiRecipes.AiRecipe,OrderAiChatRecipes.AiChatRecipe",
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
            }).ToList(),

            AiChatRecipes = subOrder.OrderAiChatRecipes.Select(a => new OrderAiChatRecipeResponse
            {
                AiChatRecipeId = a.AiChatRecipeId,
                RecipeName = a.AiChatRecipe!.RecommendedRecipeName ?? "AI Chat Recipe",
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

        if (mainOrder != null)
        {
            var patient = await _unitOfWork.PatientRepository.GetAsync(p => p.PatientId == mainOrder.PatientId, includeProperties: "User", tracked: false, cancellationToken: cancellationToken);

            if (patient != null)
            {
                string notifTitle = "Order Update 🔄";
                string notifMessage = $"Your items in Order #{mainOrder.OrderId} are now: {newSubStatus.ToString()}.";

                if (newSubStatus == SubOrderStatus.Shipped)
                {
                    notifTitle = "Order Shipped! 🚚";
                    notifMessage = $"Good news! Your items in Order #{mainOrder.OrderId} have been shipped. Tracking ID: {subOrder.ExternalDeliveryID}.";
                }
                else if (newSubStatus == SubOrderStatus.Delivered)
                {
                    notifTitle = "Order Delivered 🎉";
                    notifMessage = $"Your items in Order #{mainOrder.OrderId} have been delivered successfully. We hope you feel better soon!";
                }

                await _notificationService.SendNotificationAsync(
                    userId: patient.UserId,
                    title: notifTitle,
                    message: notifMessage,
                    cancellationToken: cancellationToken);
            }
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
        if (subStatuses == null || !subStatuses.Any())
            return OrderStatus.Pending.ToString();

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

        if (subStatuses.Contains(SubOrderStatus.Cancelled.ToString()))
            return OrderStatus.PartiallyCancelled.ToString();

        return OrderStatus.Pending.ToString();
    }

    public async Task<HerbalistFinancialDashboardResponse> GetHerbalistFinancialsAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId))
            return new HerbalistFinancialDashboardResponse();

        var herbalist = await _unitOfWork.HerbalistRepository.GetAsync(
            h => h.UserId == parsedUserId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            return new HerbalistFinancialDashboardResponse();

        var subOrders = await _unitOfWork.SubOrderRepository.GetAllAsync(
            filter: s => s.HerbalistId == herbalist.HerbalistId,
            includeProperties: "Order,OrderHerbs.Herb,OrderRecipes.Recipe,OrderAiRecipes.AiRecipe,OrderAiChatRecipes.AiChatRecipe", // 👈 ضفنا الـ AI Recipe هنا
            tracked: false,
            cancellationToken: cancellationToken);

        if (!subOrders.Any())
            return new HerbalistFinancialDashboardResponse();

        string cancelledStatus = SubOrderStatus.Cancelled.ToString();

        return new HerbalistFinancialDashboardResponse
        {
            CurrentBalance = subOrders
                .Where(s => s.Status != cancelledStatus)
                .Sum(s => s.SubTotal),

            CancelledDeductions = subOrders
                .Where(s => s.Status == cancelledStatus)
                .Sum(s => s.SubTotal),

            TasksHistory = subOrders.Select(s => new TaskHistoryResponse
            {
                TaskId = s.SubOrderId,
                ProductName = DetermineProductName(s), 
                Amount = s.SubTotal,
                Status = s.Status,
                Date = s.Order?.OrderDate
            }).OrderByDescending(t => t.Date) 
        };
    }

    private string DetermineProductName(SubOrder subOrder)
    {
        string productName = "Unknown Product";

        if (subOrder.OrderHerbs != null && subOrder.OrderHerbs.Any())
        {
            productName = subOrder.OrderHerbs.First().Herb?.HerbName ?? "Medicinal Herb";
        }
        else if (subOrder.OrderRecipes != null && subOrder.OrderRecipes.Any())
        {
            productName = subOrder.OrderRecipes.First().Recipe?.Description ?? "Herbal Recipe";
        }
        else if (subOrder.OrderAiRecipes != null && subOrder.OrderAiRecipes.Any())
        {
            productName = subOrder.OrderAiRecipes.First().AiRecipe?.RecommendedRecipeName ?? "AI Generated Recipe";
        }
        else if (subOrder.OrderAiChatRecipes != null && subOrder.OrderAiChatRecipes.Any())
        {
            productName = subOrder.OrderAiChatRecipes.First().AiChatRecipe?.RecommendedRecipeName ?? "AI Chat Recipe";
        }

        int maxLength = 35;
        if (productName.Length > maxLength)
        {
            productName = productName.Substring(0, maxLength) + "...";
        }

        return productName;
    }

    public async Task CancelSubOrderByPatientAsync(int subOrderId, string userId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId))
            throw new UnauthorizedAccessException("User is unidentified or the session has expired.");

        var patient = await _unitOfWork.PatientRepository.GetAsync(
            p => p.UserId == parsedUserId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (patient == null)
            throw new UnauthorizedAccessException("Patient profile not found in the system.");

        var subOrder = await _unitOfWork.SubOrderRepository.GetAsync(
            filter: s => s.SubOrderId == subOrderId,
            tracked: true,
            includeProperties: "Order.SubOrders",
            cancellationToken: cancellationToken);

        if (subOrder == null)
            throw new KeyNotFoundException("Sub-order not found.");

        if (subOrder.Order.PatientId != patient.PatientId)
            throw new UnauthorizedAccessException("You do not have permission to cancel this sub-order.");

        if (subOrder.Status != SubOrderStatus.Pending.ToString() && subOrder.Status != "AwaitingPayment")
        {
            throw new InvalidOperationException("You cannot cancel this sub-order because the herbalist has already started preparing or has shipped it.");
        }

        subOrder.Status = SubOrderStatus.Cancelled.ToString();
        _unitOfWork.SubOrderRepository.Update(subOrder);

        if (subOrder.Order != null)
        {
            var allSubStatuses = subOrder.Order.SubOrders.Select(s => s.Status).ToList();

            subOrder.Order.OrderStatus = DetermineMainOrderStatus(allSubStatuses);
            _unitOfWork.OrderRepository.Update(subOrder.Order);
        }

        var herbalist = await _unitOfWork.HerbalistRepository.GetAsync(h => h.HerbalistId == subOrder.HerbalistId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist != null)
        {
            await notificationService.SendNotificationAsync(
                userId: herbalist.UserId,
                title: "Order Cancelled 🚫",
                message: $"The patient has cancelled their items in Order #{subOrder.Order!.OrderId}. No further action is needed.",
                cancellationToken: cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}