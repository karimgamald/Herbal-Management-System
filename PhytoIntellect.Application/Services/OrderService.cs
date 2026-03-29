using AutoMapper;
using PhytoIntellect.Application.Contracts.Orders;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;


namespace PhytoIntellect.Application.Services;

public class OrderService(IUnitOfWork unitOfWork, IMapper mapper) : IOrderService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    // --- 1. إنشاء الطلب ---
    public async Task<string> CreateOrderAsync(string userId, CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId))
            throw new Exception("Invalid User ID format.");

        var patient = await _unitOfWork.PatientRepository.GetAsync(
            p => p.UserId == parsedUserId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (patient == null)
            throw new Exception("Patient not found.");

        int patientId = patient.PatientId;

        // =============================
        // ✅ Address Handling
        // =============================
        string finalShippingAddress = request.ShippingAddress;

        if (string.IsNullOrWhiteSpace(finalShippingAddress) || finalShippingAddress.Trim().ToLower() == "string")
        {
            var userEntity = await _unitOfWork.UserRepository.GetAsync(
                u => u.Id == parsedUserId,
                tracked: false,
                cancellationToken: cancellationToken);

            if (userEntity != null)
            {
                var parts = new List<string>();

                if (!string.IsNullOrWhiteSpace(userEntity.Governorate))
                    parts.Add(userEntity.Governorate);

                if (!string.IsNullOrWhiteSpace(userEntity.City))
                    parts.Add(userEntity.City);

                if (!string.IsNullOrWhiteSpace(userEntity.Street))
                    parts.Add(userEntity.Street);

                finalShippingAddress = string.Join(", ", parts);
            }

            if (string.IsNullOrWhiteSpace(finalShippingAddress))
                throw new InvalidOperationException("Shipping address is missing.");
        }

        // =============================
        // ✅ Filter invalid data (🔥 مهم)
        // =============================
        var validRecipes = request.Recipes?
            .Where(r => r.RecipeId > 0 && r.Quantity > 0)
            .ToList();

        var validHerbs = request.Herbs?
            .Where(h => h.HerbId > 0 && h.Quantity > 0)
            .ToList();

        // =============================
        // ✅ Prevent empty order
        // =============================
        if ((validRecipes == null || !validRecipes.Any()) &&
            (validHerbs == null || !validHerbs.Any()))
        {
            throw new InvalidOperationException("Order must contain at least one herb or recipe.");
        }

        var mainOrder = new Order
        {
            PatientId = patientId,
            ShippingAddress = finalShippingAddress,
            PaymentMethod = request.PaymentMethod,
            OrderDate = DateTime.UtcNow,
            OrderStatus = "Pending",
            SubOrders = new List<SubOrder>()
        };

        // =============================
        // ✅ Recipes
        // =============================
        if (validRecipes != null && validRecipes.Any())
        {
            var recipeIds = validRecipes.Select(r => r.RecipeId).ToList();

            var recipesFromDb = await _unitOfWork.RecipeRepository
                .GetAllAsync(r => recipeIds.Contains(r.RecipeId));

            if (recipesFromDb.Count() != recipeIds.Count)
                throw new InvalidOperationException("Some recipes do not exist.");

            if (recipesFromDb.Any(r => r.HerbalistId == null))
                throw new InvalidOperationException("Cannot order AI recipes.");

            var grouped = recipesFromDb.GroupBy(r => r.HerbalistId);

            foreach (var group in grouped)
            {
                var subOrder = new SubOrder
                {
                    HerbalistId = group.Key ?? 0,
                    Status = "Pending",
                    OrderRecipes = new List<OrderRecipe>(),
                    OrderHerbs = new List<OrderHerb>()
                };

                foreach (var recipe in group)
                {
                    var quantity = validRecipes
                        .First(r => r.RecipeId == recipe.RecipeId).Quantity;

                    decimal price = 100; // replace with actual

                    subOrder.OrderRecipes.Add(new OrderRecipe
                    {
                        RecipeId = recipe.RecipeId,
                        Quantity = quantity,
                        UnitPrice = price,
                        SubTotal = price * quantity
                    });
                }

                mainOrder.SubOrders.Add(subOrder);
            }
        }

        // =============================
        // ✅ Herbs (🔥 FIXED بالكامل)
        // =============================
        if (validHerbs != null && validHerbs.Any())
        {
            var herbIds = validHerbs.Select(h => h.HerbId).ToList();

            var herbsFromDb = await _unitOfWork.HerbRepository
                .GetAllAsync(h => herbIds.Contains(h.HerbId));

            if (herbsFromDb.Count() != herbIds.Count)
                throw new InvalidOperationException("Some herbs do not exist.");

            var grouped = validHerbs.GroupBy(h => h.HerbalistId);

            foreach (var group in grouped)
            {
                int herbalistId = (int)group.Key;

                var subOrder = mainOrder.SubOrders
                    .FirstOrDefault(s => s.HerbalistId == herbalistId);

                if (subOrder == null)
                {
                    subOrder = new SubOrder
                    {
                        HerbalistId = herbalistId,
                        Status = "Pending",
                        OrderRecipes = new List<OrderRecipe>(),
                        OrderHerbs = new List<OrderHerb>()
                    };

                    mainOrder.SubOrders.Add(subOrder);
                }

                foreach (var item in group)
                {
                    var dbHerb = await _unitOfWork.HerbalistHerbRepository.GetAsync(
                        hh => hh.HerbId == item.HerbId &&
                              hh.HerbalistId == herbalistId &&
                              hh.IsActive,
                        tracked: false,
                        cancellationToken: cancellationToken);

                    if (dbHerb == null || dbHerb.Price == null)
                        throw new InvalidOperationException($"Herb {item.HerbId} not available for herbalist {herbalistId}.");

                    subOrder.OrderHerbs.Add(new OrderHerb
                    {
                        HerbId = item.HerbId,
                        Quantity = item.Quantity,
                        UnitPrice = dbHerb.Price.Value,
                        SubTotal = dbHerb.Price.Value * item.Quantity
                    });
                }
            }
        }

        // =============================
        // ✅ Final Calculation
        // =============================
        foreach (var sub in mainOrder.SubOrders)
        {
            sub.SubTotal =
                (sub.OrderRecipes?.Sum(r => r.SubTotal) ?? 0) +
                (sub.OrderHerbs?.Sum(h => h.SubTotal) ?? 0);
        }

        mainOrder.ItemsTotal = mainOrder.SubOrders.Sum(s => s.SubTotal);
        mainOrder.DeliveryFee = 50;
        mainOrder.TotalPrice = mainOrder.ItemsTotal + mainOrder.DeliveryFee;

        await _unitOfWork.OrderRepository.CreateAsync(mainOrder);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return "Order created successfully!";
    }

    // --- 2. المريض بيشوف لستة طلباته ---
    public async Task<IEnumerable<OrderSummaryResponse>> GetPatientOrdersAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId)) return Enumerable.Empty<OrderSummaryResponse>();

        var patient = await _unitOfWork.PatientRepository.GetAsync(p => p.UserId == parsedUserId, tracked: false, cancellationToken: cancellationToken);
        if (patient == null) 
            return Enumerable.Empty<OrderSummaryResponse>();

        var orders = await _unitOfWork.OrderRepository.GetAllAsync(
            filter: o => o.PatientId == patient.PatientId,
            tracked: false,
            cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<OrderSummaryResponse>>(orders);
    }

    // --- 3. المريض بيشوف تفاصيل الطلب ---
    public async Task<OrderDetailsResponse?> GetOrderDetailsForPatientAsync(int orderId, string userId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId)) return null;

        var patient = await _unitOfWork.PatientRepository.GetAsync(p => p.UserId == parsedUserId, tracked: false, cancellationToken: cancellationToken);
        if (patient == null) 
            return null;

        var order = await _unitOfWork.OrderRepository.GetAsync(
            filter: o => o.OrderId == orderId && o.PatientId == patient.PatientId,
            // 🎯 ضفنا اسم الجدول بتاع العطار هنا عشان الـ AutoMapper يعرف يجيب اسمه (عدلها حسب اسم الـ Navigation Property عندك)
            includeProperties: "SubOrders.Herbalist.User",
            tracked: false,
            cancellationToken: cancellationToken);

        if (order == null) 
            return null;

        return _mapper.Map<OrderDetailsResponse>(order);
    }

    // --- 4. المريض بيلغي الطلب ---
    public async Task CancelOrderAsync(int orderId, string userId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId)) throw new Exception("Invalid User ID.");

        var patient = await _unitOfWork.PatientRepository.GetAsync(p => p.UserId == parsedUserId, tracked: false, cancellationToken: cancellationToken);
        if (patient == null) 
            throw new Exception("Patient not found.");

        var order = await _unitOfWork.OrderRepository.GetAsync(
            filter: o => o.OrderId == orderId && o.PatientId == patient.PatientId,
            includeProperties: "SubOrders",
            tracked: true,
            cancellationToken: cancellationToken);

        if (order == null) 
            throw new Exception("Order not found.");

        if (order.SubOrders.Any(s => s.Status != "Pending" && s.Status != "Accepted"))
            throw new Exception("Cannot cancel order because some items are already being prepared or shipped.");

        // نرجع الكميات للمخزن تاني لو الأوردر اتلغى (دي ممكن تعملها خطوة إضافية بعدين للبيزنس)

        order.OrderStatus = "Cancelled";
        foreach (var subOrder in order.SubOrders)
        {
            subOrder.Status = "Cancelled";
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}