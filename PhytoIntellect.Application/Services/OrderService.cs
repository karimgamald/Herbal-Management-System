using AutoMapper;
using AutoMapper.QueryableExtensions;
using PhytoIntellect.Application.Contracts.Orders;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Constants;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Enums;

namespace PhytoIntellect.Application.Services;

public class OrderService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService) : IOrderService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<string> CreateOrderAsync(string userId, CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        int parsedUserId = ParseUserId(userId);
        int patientId = await GetPatientIdAsync(parsedUserId, cancellationToken);

        string finalShippingAddress = 
            await ResolveShippingAddressAsync(parsedUserId, request.ShippingAddress, cancellationToken);

        var (orderStatus, subOrderStatus) = DetermineOrderStatuses(request.PaymentMethod);

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
        if (request.AiChatRecipes != null && request.AiChatRecipes.Any())
        {
            await ProcessAiChatRecipesAsync(request.AiChatRecipes, mainOrder, subOrderStatus, cancellationToken);
        }

        CalculateOrderTotals(mainOrder);
        await _unitOfWork.OrderRepository.CreateAsync(mainOrder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (mainOrder.SubOrders != null && mainOrder.SubOrders.Any())
        {
            foreach (var subOrder in mainOrder.SubOrders)
            {
                var herbalistEntity = await _unitOfWork.HerbalistRepository.GetAsync(
                    filter: h => h.HerbalistId == subOrder.HerbalistId,
                    tracked: false,
                    cancellationToken: cancellationToken);

                if (herbalistEntity != null)
                {
                    int itemsCount = (subOrder.OrderHerbs?.Count ?? 0) +
                                     (subOrder.OrderRecipes?.Count ?? 0) +
                                     (subOrder.OrderAiRecipes?.Count ?? 0) +
                                     (subOrder.OrderAiChatRecipes?.Count ?? 0);

                    string paymentText = mainOrder.PaymentMethod == "Cash" ? "Cash on Delivery" : "Paid via Card";

                    await notificationService.SendNotificationAsync(
                        userId: herbalistEntity.UserId,
                        title: "New Order Alert! 📦",
                        message: $"You received a new order #{mainOrder.OrderId} containing {itemsCount} items. Payment Method: {paymentText}. Please start preparing it.",
                        cancellationToken: cancellationToken);
                }
            }
        }

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

        var query = unitOfWork.OrderRepository
            .GetQueryable(tracked: false)
            .Where(o => o.PatientId == patient.PatientId);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();

            query = query.Where(o =>
                o.OrderId.ToString().Contains(search));
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
                    ? query.OrderByDescending(o => o.OrderId)
                    : query.OrderBy(o => o.OrderId),

                "date" => isDesc
                    ? query.OrderByDescending(o => o.OrderDate)
                    : query.OrderBy(o => o.OrderDate),

                _ => isDesc
                    ? query.OrderByDescending(o => o.OrderDate)
                    : query.OrderBy(o => o.OrderDate)
            };
        }
        else
        {
            query = query.OrderByDescending(o => o.OrderDate);
        }

        var projectedQuery = query.ProjectTo<OrderSummaryResponse>(
            mapper.ConfigurationProvider);

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
            includeProperties: "SubOrders.Herbalist.User,SubOrders.OrderRecipes.Recipe,SubOrders.OrderHerbs.Herb,SubOrders.OrderAiRecipes.AiRecipe,SubOrders.OrderAiChatRecipes.AiChatRecipe",
            tracked: false,
            cancellationToken: cancellationToken);

        if (order == null) return null;

        return _mapper.Map<OrderDetailsResponse>(order);
    }

    public async Task CancelOrderAsync(int orderId, string userId, string userRole, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId))
            throw new Exception("Invalid User ID.");

        Order? order = null;

        // 1. إذا كان المستخدم "أدمن"، جلب الطلب مباشرة بدون قيود الملكية
        if (userRole == AppRoles.Admin) // تأكد من مطابقة اسم الـ Constant للأدمن لديك
        {
            order = await _unitOfWork.OrderRepository.GetAsync(
                filter: o => o.OrderId == orderId,
                includeProperties: "SubOrders",
                tracked: true,
                cancellationToken: cancellationToken);
        }
        //  2. إذا كان المستخدم مريضاً، تحقق من ملكيته للطلب
        else
        {
            var patient = await _unitOfWork.PatientRepository.GetAsync(
                p => p.UserId == parsedUserId,
                tracked: false,
                cancellationToken: cancellationToken);

            if (patient == null)
                throw new Exception("Patient profile not found.");

            order = await _unitOfWork.OrderRepository.GetAsync(
                filter: o => o.OrderId == orderId && o.PatientId == patient.PatientId,
                includeProperties: "SubOrders",
                tracked: true,
                cancellationToken: cancellationToken);
        }

        // 3. التحقق من وجود الطلب
        if (order == null)
            throw new Exception("Order not found or you don't have permission to access it.");

        // 4. التحقق من حالة الطلب الفرعي (الـ Business Logic الخاص بك)
        if (order.SubOrders.Any(s => s.Status != SubOrderStatus.Pending.ToString()))
            throw new Exception("Cannot cancel order because some items are already being prepared or shipped.");

        // 5. تحديث الحالات إلى ملغى
        order.OrderStatus = OrderStatus.Cancelled.ToString();

        foreach (var subOrder in order.SubOrders)
        {
            subOrder.Status = SubOrderStatus.Cancelled.ToString();
        }

        // 6. حفظ التغييرات
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

        if (order.SubOrders != null)
        {
            foreach (var subOrder in order.SubOrders)
            {
                var herbalist = await _unitOfWork.HerbalistRepository.GetAsync(h => h.HerbalistId == subOrder.HerbalistId, tracked: false, cancellationToken: cancellationToken);
                if (herbalist != null)
                {
                    await notificationService.SendNotificationAsync(
                        userId: herbalist.UserId,
                        title: "Payment Received 💳",
                        message: $"Order #{order.OrderId} has been successfully paid by the patient. You can now start preparing the items.",
                        cancellationToken: cancellationToken);
                }
            }
        }

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

        var query = unitOfWork.OrderRepository
            .GetQueryable(tracked: false)
            .Where(o => o.PatientId == patient.PatientId && o.IsFavorite);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();

            query = query.Where(o =>
                o.OrderId.ToString().Contains(search));
        }

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

        var projectedQuery = query.ProjectTo<OrderSummaryResponse>(
            mapper.ConfigurationProvider); 

        var result = await PaginatedList<OrderSummaryResponse>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);

        return result;
    }

    public async Task<PaginatedList<OrderSummaryResponse>> GetAllPendingOrdersForAdminAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.OrderRepository
            .GetQueryable(tracked: false)
            .Where(o => o.OrderStatus == OrderStatus.Pending.ToString());

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(o => o.OrderId.ToString().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = string.Equals(filters.SortDirection, "DESC", StringComparison.OrdinalIgnoreCase);
            query = filters.SortColumn.ToLower() switch
            {
                "id" => isDesc ? query.OrderByDescending(o => o.OrderId) : query.OrderBy(o => o.OrderId),
                _ => isDesc ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate)
            };
        }
        else
        {
            query = query.OrderBy(o => o.OrderDate);
        }

        var projectedQuery = query.ProjectTo<OrderSummaryResponse>(mapper.ConfigurationProvider);

        return await PaginatedList<OrderSummaryResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }
    public async Task<PaginatedList<OrderSummaryResponse>> GetAllOrdersForAdminAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.OrderRepository.GetQueryable(tracked: false);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(o => o.OrderId.ToString().Contains(search) ||
                                     o.OrderStatus.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = string.Equals(filters.SortDirection, "DESC", StringComparison.OrdinalIgnoreCase);

            query = filters.SortColumn.ToLower() switch
            {
                "id" => isDesc ? query.OrderByDescending(o => o.OrderId) : query.OrderBy(o => o.OrderId),
                "date" => isDesc ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate),
                "status" => isDesc ? query.OrderByDescending(o => o.OrderStatus) : query.OrderBy(o => o.OrderStatus),
                _ => isDesc ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate)
            };
        }
        else
        { 
            query = query.OrderByDescending(o => o.OrderDate);
        }

        var projectedQuery = query.ProjectTo<OrderSummaryResponse>(mapper.ConfigurationProvider);

        return await PaginatedList<OrderSummaryResponse>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);
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

    private async Task ProcessAiChatRecipesAsync(IEnumerable<OrderAiChatRecipeRequest> requestedAiChatRecipes, Order mainOrder, string status, CancellationToken cancellationToken)
    {
        var aiChatRecipesGrouped = requestedAiChatRecipes.GroupBy(r => r.HerbalistId);

        foreach (var group in aiChatRecipesGrouped)
        {
            int currentHerbalistId = group.Key;
            var subOrder = GetOrCreateSubOrder(mainOrder, currentHerbalistId, status);

            foreach (var requestedRecipe in group)
            {
                var inventoryItem = await _unitOfWork.HerbalistAiChatRecipeRepository.GetAsync(
                    filter: h => h.HerbalistId == currentHerbalistId
                              && h.AiChatRecipeId == requestedRecipe.AiChatRecipeId
                              && h.IsActive == true,
                    tracked: false,
                    cancellationToken: cancellationToken);

                if (inventoryItem == null)
                    throw new InvalidOperationException($"AI Chat Recipe ID {requestedRecipe.AiChatRecipeId} is not active or not sold by Herbalist ID {currentHerbalistId}.");

                decimal unitPrice = inventoryItem.Price;
                decimal itemTotal = unitPrice * requestedRecipe.Quantity;

                subOrder.OrderAiChatRecipes.Add(new OrderAiChatRecipe
                {
                    AiChatRecipeId = requestedRecipe.AiChatRecipeId,
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
            (subOrder.OrderAiRecipes?.Sum(a => a.SubTotal) ?? 0) +
            (subOrder.OrderAiChatRecipes?.Sum(c => c.SubTotal) ?? 0);
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


