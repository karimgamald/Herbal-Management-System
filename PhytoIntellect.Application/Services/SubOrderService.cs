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

public class SubOrderService(IUnitOfWork unitOfWork, IMapper mapper) : ISubOrderService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

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

        // 🔥 Query
        var query = unitOfWork.SubOrderRepository
            .GetQueryable(tracked: false)
            .Where(s => s.HerbalistId == herbalist.HerbalistId);

        // 🔍 Search
        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();

            query = query.Where(s =>
                s.Herbalist.User.FullName.ToLower().Contains(search));
        }

        // 🔃 Sorting
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
                    ? query.OrderByDescending(s => s.Order.OrderDate)
                    : query.OrderBy(s => s.Order.OrderDate),

                _ => isDesc
                    ? query.OrderByDescending(s => s.SubOrderId)
                    : query.OrderBy(s => s.SubOrderId)
            };
        }
        else
        {
            // الافتراضي: الأحدث أولاً
            query = query.OrderByDescending(s => s.Order.OrderDate);
        }

        // 🚀 Projection
        var projectedQuery = query.ProjectTo<SubOrderSummaryResponse>(
            mapper.ConfigurationProvider);

        // 📄 Pagination
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

    public async Task<HerbalistFinancialDashboardResponse> GetHerbalistFinancialsAsync(string userId, CancellationToken cancellationToken = default)
    {
        // 1. التأكد من اليوزر والعطار
        if (!int.TryParse(userId, out int parsedUserId))
            return new HerbalistFinancialDashboardResponse();

        var herbalist = await _unitOfWork.HerbalistRepository.GetAsync(
            h => h.UserId == parsedUserId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            return new HerbalistFinancialDashboardResponse();

        // 2. نجيب كل الطلبات مع كل الجداول المرتبطة (عشان نعرف نجيب الأسماء والتاريخ)
        var subOrders = await _unitOfWork.SubOrderRepository.GetAllAsync(
            filter: s => s.HerbalistId == herbalist.HerbalistId,
            includeProperties: "Order,OrderHerbs.Herb,OrderRecipes.Recipe,OrderAiRecipes.AiRecipe,OrderAiChatRecipes.AiChatRecipe", // 👈 ضفنا الـ AI Recipe هنا
            tracked: false,
            cancellationToken: cancellationToken);

        if (!subOrders.Any())
            return new HerbalistFinancialDashboardResponse();

        // 3. الحسابات الوقتية (Derived State)
        string cancelledStatus = SubOrderStatus.Cancelled.ToString();

        return new HerbalistFinancialDashboardResponse
        {
            // الرصيد: أي حاجة مش ملغية
            CurrentBalance = subOrders
                .Where(s => s.Status != cancelledStatus)
                .Sum(s => s.SubTotal),

            // المخصوم: الحاجات الملغية بس
            CancelledDeductions = subOrders
                .Where(s => s.Status == cancelledStatus)
                .Sum(s => s.SubTotal),

            // كشف الحساب
            TasksHistory = subOrders.Select(s => new TaskHistoryResponse
            {
                TaskId = s.SubOrderId,
                ProductName = DetermineProductName(s), // 👈 بننده على الدالة المساعدة
                Amount = s.SubTotal,
                Status = s.Status,
                Date = s.Order?.OrderDate
            }).OrderByDescending(t => t.Date) // ترتيب من الأحدث للأقدم
        };
    }

    // Helper Method عشان نجيب اسم المنتج أياً كان نوعه
    private string DetermineProductName(SubOrder subOrder)
    {
        string productName = "Unknown Product";

        // 1. تحديد الاسم بناءً على نوع المنتج
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

        // 2. قص النص لو طويل بزيادة (الثلاث نقط الشيك)
        int maxLength = 35;
        if (productName.Length > maxLength)
        {
            productName = productName.Substring(0, maxLength) + "...";
        }

        return productName;
    }
}