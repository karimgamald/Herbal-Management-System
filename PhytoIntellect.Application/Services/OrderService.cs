using AutoMapper;
using AutoMapper.QueryableExtensions;
using PhytoIntellect.Application.Contracts.Orders;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Enums;

namespace PhytoIntellect.Application.Services;

public class OrderService(IUnitOfWork unitOfWork, IMapper mapper) : IOrderService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<string> CreateOrderAsync(string userId, CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        // 1. التحقق من المريض
        int parsedUserId = ParseUserId(userId);
        int patientId = await GetPatientIdAsync(parsedUserId, cancellationToken);

        // 2. تجهيز العنوان
        string finalShippingAddress = 
            await ResolveShippingAddressAsync(parsedUserId, request.ShippingAddress, cancellationToken);

        // 3. تحديد حالة الدفع
        var (orderStatus, subOrderStatus) = DetermineOrderStatuses(request.PaymentMethod);

        // 4. تهيئة الأوردر الرئيسي
        var mainOrder = new Order
        {
            PatientId = patientId,
            ShippingAddress = finalShippingAddress,
            PaymentMethod = request.PaymentMethod,
            OrderStatus = orderStatus,
            PaymentStatus = PaymentStatus.Pending.ToString(),
            ExternalPaymentID = null,
            OrderDate = DateTime.UtcNow,
            SubOrders = new List<SubOrder>()
        };

        // 5. معالجة العناصر (الوصفات والأعشاب)

        if (request.Herbs != null && request.Herbs.Any())
        {
            await ProcessHerbsAsync(request.Herbs, mainOrder, subOrderStatus, cancellationToken);
        }
        if (request.Recipes != null && request.Recipes.Any())
        {
            await ProcessRecipesAsync(request.Recipes, mainOrder, subOrderStatus, cancellationToken);
        }
        if (request.AiRecipes != null && request.AiRecipes.Any())
        {
            await ProcessAiRecipesAsync(request.AiRecipes, mainOrder, subOrderStatus, cancellationToken);
        }

        // 6. حساب الإجماليات والحفظ
        CalculateOrderTotals(mainOrder);
        await _unitOfWork.OrderRepository.CreateAsync(mainOrder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return mainOrder.OrderStatus == "AwaitingPayment"
            ? "Order created successfully. Please proceed to payment."
            : "Order created successfully and sent to herbalists!";
    }

    public async Task<PaginatedList<OrderSummaryResponse>> GetPatientOrdersAsync(string userId,RequestFilters filters,CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId))
            return new PaginatedList<OrderSummaryResponse>(
                new List<OrderSummaryResponse>(),
                0,
                filters.PageNumber,
                filters.PageSize);

        var patient = await unitOfWork.PatientRepository.GetAsync(
            p => p.UserId == parsedUserId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (patient == null)
            return new PaginatedList<OrderSummaryResponse>(
                new List<OrderSummaryResponse>(),
                0,
                filters.PageNumber,
                filters.PageSize);

        // 🔥 Query
        var query = unitOfWork.OrderRepository
            .GetQueryable(tracked: false)
            .Where(o => o.PatientId == patient.PatientId);

        // 🔍 Search
        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();

            query = query.Where(o =>
                o.OrderId.ToString().Contains(search));
        }

        // 🔃 Sorting
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = filters.SortDirection?.ToUpper() == "DESC";

            query = filters.SortColumn.ToLower() switch
            {
                "date" => isDesc
                    ? query.OrderByDescending(o => o.OrderDate)
                    : query.OrderBy(o => o.OrderDate),

                "id" => isDesc
                    ? query.OrderByDescending(o => o.OrderId)
                    : query.OrderBy(o => o.OrderId),

                _ => query.OrderBy(o => o.OrderId)
            };
        }
        else
        {
            query = query.OrderByDescending(o => o.OrderDate);
        }

        // 🚀 Projection
        var projectedQuery = query.ProjectTo<OrderSummaryResponse>(
            mapper.ConfigurationProvider);

        // 📄 Pagination
        var result = await PaginatedList<OrderSummaryResponse>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);

        return result;
    }

    public async Task<OrderDetailsResponse?> GetOrderDetailsForPatientAsync(int orderId, string userId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId)) return null;

        var patient = await _unitOfWork.PatientRepository.GetAsync(p => p.UserId == parsedUserId, tracked: false, cancellationToken: cancellationToken);
        if (patient == null) return null;

        var order = await _unitOfWork.OrderRepository.GetAsync(
            filter: o => o.OrderId == orderId && o.PatientId == patient.PatientId,
            includeProperties: "SubOrders.Herbalist.User,SubOrders.OrderRecipes.Recipe,SubOrders.OrderHerbs.Herb,SubOrders.OrderAiRecipes.AiRecipe",
            tracked: false,
            cancellationToken: cancellationToken);

        if (order == null) return null;

        return _mapper.Map<OrderDetailsResponse>(order);
    }

    public async Task CancelOrderAsync(int orderId, string userId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId)) throw new Exception("Invalid User ID.");

        var patient = await _unitOfWork.PatientRepository.GetAsync(p => p.UserId == parsedUserId, tracked: false, cancellationToken: cancellationToken);
        if (patient == null) throw new Exception("Patient not found.");

        var order = await _unitOfWork.OrderRepository.GetAsync(
            filter: o => o.OrderId == orderId && o.PatientId == patient.PatientId,
            includeProperties: "SubOrders",
            tracked: true,
            cancellationToken: cancellationToken);

        if (order == null) throw new Exception("Order not found.");

        if (order.SubOrders.Any(s => s.Status != SubOrderStatus.Pending.ToString()))
            throw new Exception("Cannot cancel order because some items are already being prepared or shipped.");

        order.OrderStatus = OrderStatus.Cancelled.ToString();
        foreach (var subOrder in order.SubOrders)
        {
            subOrder.Status = SubOrderStatus.Cancelled.ToString();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<string> SimulatePaymentAsync(int orderId, string userId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId))
            throw new ArgumentException("Invalid User ID format.");

        var patient = await _unitOfWork.PatientRepository.GetAsync(
            filter: p => p.UserId == parsedUserId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (patient == null)
            throw new UnauthorizedAccessException("Patient account not found.");

        var order = await _unitOfWork.OrderRepository.GetAsync(
            filter: o => o.OrderId == orderId && o.PatientId == patient.PatientId,
            includeProperties: "SubOrders",
            tracked: true,
            cancellationToken: cancellationToken);

        if (order == null)
            throw new KeyNotFoundException("Order not found or you do not have permission to pay for it.");

        if (!string.IsNullOrEmpty(order.ExternalPaymentID))
            throw new InvalidOperationException("This order is already paid.");

        string fakeTransactionId = $"TXN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

        order.ExternalPaymentID = fakeTransactionId;
        order.OrderStatus = OrderStatus.Pending.ToString();
        order.PaymentStatus = PaymentStatus.Paid.ToString();
        foreach (var subOrder in order.SubOrders)
        {
            subOrder.Status = SubOrderStatus.Pending.ToString();
        }

        _unitOfWork.OrderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return fakeTransactionId;
    }

    public async Task<bool> ToggleFavoriteOrderAsync(string userId, int orderId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId)) throw new ArgumentException("Invalid User ID.");

        var patient = await _unitOfWork.PatientRepository.GetAsync(p => p.UserId == parsedUserId, tracked: false, cancellationToken: cancellationToken);
        if (patient == null) throw new UnauthorizedAccessException("Patient not found.");

        var order = await _unitOfWork.OrderRepository.GetAsync(
            filter: o => o.OrderId == orderId && o.PatientId == patient.PatientId,
            tracked: true, 
            cancellationToken: cancellationToken);

        if (order == null) throw new KeyNotFoundException("Order not found.");

        order.IsFavorite = !order.IsFavorite;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.IsFavorite;
    }

    public async Task<PaginatedList<OrderSummaryResponse>> GetFavoriteOrdersAsync(string userId,RequestFilters filters,CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId))
            return new PaginatedList<OrderSummaryResponse>(
                new List<OrderSummaryResponse>(),
                0,
                filters.PageNumber,
                filters.PageSize);

        var patient = await unitOfWork.PatientRepository.GetAsync(
            p => p.UserId == parsedUserId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (patient == null)
            return new PaginatedList<OrderSummaryResponse>(
                new List<OrderSummaryResponse>(),
                0,
                filters.PageNumber,
                filters.PageSize);

        // 🔥 Query
        var query = unitOfWork.OrderRepository
            .GetQueryable(tracked: false)
            .Where(o => o.PatientId == patient.PatientId && o.IsFavorite);

        // 🔍 Search
        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();

            query = query.Where(o =>
                o.OrderId.ToString().Contains(search));
        }

        // 🔃 Sorting
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = filters.SortDirection?.ToUpper() == "DESC";

            query = filters.SortColumn.ToLower() switch
            {
                "date" => isDesc
                    ? query.OrderByDescending(o => o.OrderDate)
                    : query.OrderBy(o => o.OrderDate),

                "id" => isDesc
                    ? query.OrderByDescending(o => o.OrderId)
                    : query.OrderBy(o => o.OrderId),

                _ => query.OrderBy(o => o.OrderId)
            };
        }
        else
        {
            query = query.OrderByDescending(o => o.OrderDate);
        }

        // 🚀 Projection
        var projectedQuery = query.ProjectTo<OrderSummaryResponse>(
            mapper.ConfigurationProvider);

        // 📄 Pagination
        var result = await PaginatedList<OrderSummaryResponse>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);

        return result;
    }


    #region Private Helper Methods

    private int ParseUserId(string userId)
    {
        if (!int.TryParse(userId, out int parsedUserId))
            throw new ArgumentException("Invalid User ID format.");
        return parsedUserId;
    }

    private async Task<int> GetPatientIdAsync(int parsedUserId, CancellationToken cancellationToken)
    {
        var patient = await _unitOfWork.PatientRepository.GetAsync(
            filter: p => p.UserId == parsedUserId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (patient == null) throw new UnauthorizedAccessException("Patient not found.");
        return patient.PatientId;
    }

    private async Task<string> ResolveShippingAddressAsync(int parsedUserId, string requestedAddress, CancellationToken cancellationToken)
    {
        string finalAddress = requestedAddress;

        if (string.IsNullOrWhiteSpace(finalAddress) || finalAddress.Trim().ToLower() == "string")
        {
            var userEntity = await _unitOfWork.UserRepository.GetAsync(
                filter: u => u.Id == parsedUserId,
                tracked: false,
                cancellationToken: cancellationToken);

            if (userEntity != null)
            {
                var addressParts = new List<string>();
                if (!string.IsNullOrWhiteSpace(userEntity.Governorate)) addressParts.Add(userEntity.Governorate);
                if (!string.IsNullOrWhiteSpace(userEntity.City)) addressParts.Add(userEntity.City);
                if (!string.IsNullOrWhiteSpace(userEntity.Street)) addressParts.Add(userEntity.Street);

                finalAddress = string.Join(", ", addressParts);
            }

            if (string.IsNullOrWhiteSpace(finalAddress))
                throw new InvalidOperationException("Shipping address is not provided in the request, and your profile does not have a saved address.");
        }

        return finalAddress;
    }

    private (string OrderStatus, string SubOrderStatus) DetermineOrderStatuses(string paymentMethod)
    {
        if (!Enum.TryParse<PaymentMethod>(paymentMethod, true, out var selectedPayment))
            throw new InvalidOperationException("Invalid Payment Method.");

        string orderStatus = selectedPayment == PaymentMethod.Cash ? OrderStatus.Pending.ToString() : "AwaitingPayment";
        string subOrderStatus = selectedPayment == PaymentMethod.Cash ? SubOrderStatus.Pending.ToString() : "AwaitingPayment";

        return (orderStatus, subOrderStatus);
    }

    private async Task ProcessRecipesAsync(IEnumerable<OrderRecipeRequest> requestedRecipes, Order mainOrder, string status, CancellationToken cancellationToken)
    {
        var recipeIds = requestedRecipes.Select(r => r.RecipeId).ToList();
        var recipesFromDb = await _unitOfWork.RecipeRepository.GetAllAsync(r => recipeIds.Contains(r.RecipeId));

        if (recipesFromDb.Any(r => r.HerbalistId == null))
            throw new InvalidOperationException("Cannot order AI recipes directly. Please order the herbs individually.");

        var recipesGrouped = recipesFromDb.GroupBy(r => r.HerbalistId);

        foreach (var group in recipesGrouped)
        {
            int currentHerbalistId = group.Key ?? 0;
            var subOrder = GetOrCreateSubOrder(mainOrder, currentHerbalistId, status);

            foreach (var recipe in group)
            {
                var quantity = requestedRecipes.First(r => r.RecipeId == recipe.RecipeId).Quantity;
                decimal unitPrice = recipe.Price > 0 ? recipe.Price : 100; // ToDo: AI Pricing

                subOrder.OrderRecipes.Add(new OrderRecipe
                {
                    RecipeId = recipe.RecipeId,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    SubTotal = unitPrice * quantity
                });
            }

            UpdateSubOrderTotal(subOrder);
        }
    }

    private async Task ProcessHerbsAsync(IEnumerable<OrderHerbRequest> requestedHerbs, Order mainOrder, string status, CancellationToken cancellationToken)
    {
        var herbsGrouped = requestedHerbs.GroupBy(h => h.HerbalistId);

        foreach (var group in herbsGrouped)
        {
            int currentHerbalistId = group.Key;
            var subOrder = GetOrCreateSubOrder(mainOrder, currentHerbalistId, status);

            foreach (var requestedHerb in group)
            {
                var herbalistHerbFromDb = await _unitOfWork.HerbalistHerbRepository.GetAsync(
                    filter: hh => hh.HerbId == requestedHerb.HerbId
                               && hh.HerbalistId == currentHerbalistId
                               && hh.IsActive == true,
                    tracked: false,
                    cancellationToken: cancellationToken);

                if (herbalistHerbFromDb == null)
                    throw new InvalidOperationException($"Herb ID {requestedHerb.HerbId} is not active or not sold by Herbalist ID {currentHerbalistId}.");

                if (herbalistHerbFromDb.Price == null)
                    throw new InvalidOperationException($"Price is not set for Herb ID {requestedHerb.HerbId}.");

                decimal unitPrice = herbalistHerbFromDb.Price.Value;

                decimal itemTotal = (requestedHerb.QuantityPerGram / 1000m) * unitPrice;

                subOrder.OrderHerbs.Add(new OrderHerb
                {
                    HerbId = requestedHerb.HerbId,
                    Quantity = requestedHerb.QuantityPerGram,
                    UnitPrice = unitPrice,
                    SubTotal = itemTotal
                });
            }

            UpdateSubOrderTotal(subOrder);
        }
    }

    private async Task ProcessAiRecipesAsync(IEnumerable<OrderAiRecipeRequest> requestedAiRecipes, Order mainOrder, string status, CancellationToken cancellationToken)
    {
        var aiRecipesGrouped = requestedAiRecipes.GroupBy(r => r.HerbalistId);

        foreach (var group in aiRecipesGrouped)
        {
            int currentHerbalistId = group.Key;
            var subOrder = GetOrCreateSubOrder(mainOrder, currentHerbalistId, status);

            foreach (var requestedRecipe in group)
            {
                var inventoryItem = await _unitOfWork.HerbalistAiRecipeRepository.GetAsync(
                    filter: h => h.HerbalistId == currentHerbalistId
                              && h.AiRecipeId == requestedRecipe.AiRecipeId
                              && h.IsActive == true,
                    tracked: false,
                    cancellationToken: cancellationToken);

                if (inventoryItem == null)
                    throw new InvalidOperationException($"AI Recipe ID {requestedRecipe.AiRecipeId} is not active or not sold by Herbalist ID {currentHerbalistId}.");

                decimal unitPrice = inventoryItem.Price;
                decimal itemTotal = unitPrice * requestedRecipe.Quantity;

                subOrder.OrderAiRecipes.Add(new OrderAiRecipe
                {
                    AiRecipeId = requestedRecipe.AiRecipeId,
                    Quantity = requestedRecipe.Quantity,
                    UnitPrice = unitPrice,
                    SubTotal = itemTotal
                });
            }

            UpdateSubOrderTotal(subOrder);
        }
    }

    private SubOrder GetOrCreateSubOrder(Order mainOrder, int herbalistId, string status)
    {
        var existingSubOrder = mainOrder.SubOrders.FirstOrDefault(s => s.HerbalistId == herbalistId);
        if (existingSubOrder != null) return existingSubOrder;

        var newSubOrder = new SubOrder
        {
            HerbalistId = herbalistId,
            Status = status,
            ExternalDeliveryID = null,
            OrderRecipes = new List<OrderRecipe>(),
            OrderHerbs = new List<OrderHerb>(),
            OrderAiRecipes = new List<OrderAiRecipe>()
        };
        mainOrder.SubOrders.Add(newSubOrder);
        return newSubOrder;
    }

    private void UpdateSubOrderTotal(SubOrder subOrder)
    {
        subOrder.SubTotal =
            (subOrder.OrderRecipes?.Sum(r => r.SubTotal) ?? 0) +
            (subOrder.OrderHerbs?.Sum(h => h.SubTotal) ?? 0) +
            (subOrder.OrderAiRecipes?.Sum(a => a.SubTotal) ?? 0);
    }

    private void CalculateOrderTotals(Order mainOrder)
    {
        var random = new Random();
        mainOrder.ItemsTotal = mainOrder.SubOrders.Sum(s => s.SubTotal);
        mainOrder.DeliveryFee = random.Next(40, 151);
        mainOrder.TotalPrice = mainOrder.ItemsTotal + mainOrder.DeliveryFee;
    }

    #endregion
}


