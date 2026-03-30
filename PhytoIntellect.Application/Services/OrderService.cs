using AutoMapper;
using PhytoIntellect.Application.Contracts.Orders;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Enums;

namespace PhytoIntellect.Application.Services;

public class OrderService(IUnitOfWork unitOfWork, IMapper mapper) : IOrderService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    // --- 1. إنشاء الطلب ---
    public async Task<string> CreateOrderAsync(string userId, CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId)) throw new Exception("Invalid User ID format.");

        var patient = await _unitOfWork.PatientRepository.GetAsync(
            filter: p => p.UserId == parsedUserId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (patient == null) throw new Exception("Patient not found.");
        int patientId = patient.PatientId;

        // --- سحب العنوان ---
        string finalShippingAddress = request.ShippingAddress;
        if (string.IsNullOrWhiteSpace(finalShippingAddress) || finalShippingAddress.Trim().ToLower() == "string")
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

                finalShippingAddress = string.Join(", ", addressParts);
            }

            if (string.IsNullOrWhiteSpace(finalShippingAddress))
            {
                throw new InvalidOperationException("Shipping address is not provided in the request, and your profile does not have a saved address.");
            }
        }

        // 🎯 معالجة الدفع باستخدام الـ Enums
        if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var selectedPayment))
            throw new InvalidOperationException("Invalid Payment Method.");

        var paymentStatus = PaymentStatus.Pending;
        var orderStatus = OrderStatus.Pending; // الأوردر دايماً بيبدأ Pending

        // 🎯 المحاكاة: لو فيزا أو محفظة، هنستنى ثانيتين والفلوس تعتبر اتدفعت
        if (selectedPayment == PaymentMethod.CreditCard || selectedPayment == PaymentMethod.Wallet)
        {
            await Task.Delay(10000, cancellationToken);
            paymentStatus = PaymentStatus.Paid;
        }

        var mainOrder = new Order
        {
            PatientId = patientId,
            ShippingAddress = finalShippingAddress,
            PaymentMethod = selectedPayment.ToString(),
            OrderStatus = orderStatus.ToString(),
            PaymentStatus = paymentStatus.ToString(), // 🚨 شيل الكومنت لو ضفت العمود في الداتابيز
            OrderDate = DateTime.UtcNow,
            SubOrders = new List<SubOrder>()
        };

        // --- معالجة الوصفات ---
        if (request.Recipes != null && request.Recipes.Any())
        {
            var recipeIds = request.Recipes.Select(r => r.RecipeId).ToList();
            var recipesFromDb = await _unitOfWork.RecipeRepository.GetAllAsync(r => recipeIds.Contains(r.RecipeId));

            if (recipesFromDb.Any(r => r.HerbalistId == null))
            {
                throw new InvalidOperationException("Cannot order AI recipes directly. Please order the herbs individually.");
            }

            var recipesGroupedByHerbalist = recipesFromDb.GroupBy(r => r.HerbalistId);

            foreach (var group in recipesGroupedByHerbalist)
            {
                int currentHerbalistId = group.Key ?? 0;

                var subOrder = new SubOrder
                {
                    HerbalistId = currentHerbalistId,
                    Status = SubOrderStatus.Pending.ToString(), // 👈 استخدام الـ Enum
                    ExternalDeliveryID = null,
                    OrderRecipes = new List<OrderRecipe>(),
                    OrderHerbs = new List<OrderHerb>()
                };

                foreach (var recipe in group)
                {
                    var quantity = request.Recipes.First(r => r.RecipeId == recipe.RecipeId).Quantity;

                    decimal unitPrice = recipe.Price > 0 ? recipe.Price : 100; // ToDo ai لسه على ما ندخل ال 

                    decimal itemTotal = unitPrice * quantity;

                    subOrder.OrderRecipes.Add(new OrderRecipe
                    {
                        RecipeId = recipe.RecipeId,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        SubTotal = itemTotal
                    });
                }

                subOrder.SubTotal = subOrder.OrderRecipes.Sum(r => r.SubTotal);
                mainOrder.SubOrders.Add(subOrder);
            }
        }

        // --- معالجة الأعشاب ---
        if (request.Herbs != null && request.Herbs.Any())
        {
            var herbsGroupedByHerbalist = request.Herbs.GroupBy(h => h.HerbalistId);

            foreach (var group in herbsGroupedByHerbalist)
            {
                int currentHerbalistId = group.Key;

                var existingSubOrder = mainOrder.SubOrders.FirstOrDefault(s => s.HerbalistId == currentHerbalistId);
                var subOrder = existingSubOrder ?? new SubOrder
                {
                    HerbalistId = currentHerbalistId,
                    Status = SubOrderStatus.Pending.ToString(), // 👈 استخدام الـ Enum
                    ExternalDeliveryID = null,
                    OrderRecipes = new List<OrderRecipe>(),
                    OrderHerbs = new List<OrderHerb>()
                };

                foreach (var requestedHerb in group)
                {
                    var herbalistHerbFromDb = await _unitOfWork.HerbalistHerbRepository.GetAsync(
                        filter: hh => hh.HerbId == requestedHerb.HerbId
                                   && hh.HerbalistId == currentHerbalistId
                                   && hh.IsActive == true,
                        tracked: false,
                        cancellationToken: cancellationToken);

                    if (herbalistHerbFromDb == null) // ToDo
                        throw new InvalidOperationException($"Herb ID {requestedHerb.HerbId} is not active or not sold by Herbalist ID {currentHerbalistId}.");

                    if (herbalistHerbFromDb.Price == null)
                        throw new InvalidOperationException($"Price is not set for Herb ID {requestedHerb.HerbId} by Herbalist ID {currentHerbalistId}.");

                    decimal unitPrice = herbalistHerbFromDb.Price.Value;
                    decimal itemTotal = unitPrice * requestedHerb.Quantity;

                    subOrder.OrderHerbs.Add(new OrderHerb
                    {
                        HerbId = requestedHerb.HerbId,
                        Quantity = requestedHerb.Quantity,
                        UnitPrice = unitPrice,
                        SubTotal = itemTotal
                    });
                }

                subOrder.SubTotal = subOrder.OrderRecipes.Sum(r => r.SubTotal) + subOrder.OrderHerbs.Sum(h => h.SubTotal);

                if (existingSubOrder == null)
                {
                    mainOrder.SubOrders.Add(subOrder);
                }
            }
        }

        var random = new Random();

        mainOrder.ItemsTotal = mainOrder.SubOrders.Sum(s => s.SubTotal);
        mainOrder.DeliveryFee = random.Next(40, 101);
        mainOrder.TotalPrice = mainOrder.ItemsTotal + mainOrder.DeliveryFee;

        await _unitOfWork.OrderRepository.CreateAsync(mainOrder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return "Order created successfully!";
    }

    // --- 2. المريض بيشوف لستة طلباته ---
    public async Task<IEnumerable<OrderSummaryResponse>> GetPatientOrdersAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId)) return Enumerable.Empty<OrderSummaryResponse>();

        var patient = await _unitOfWork.PatientRepository.GetAsync(p => p.UserId == parsedUserId, tracked: false, cancellationToken: cancellationToken);
        if (patient == null) return Enumerable.Empty<OrderSummaryResponse>();

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
        if (patient == null) return null;

        var order = await _unitOfWork.OrderRepository.GetAsync(
            filter: o => o.OrderId == orderId && o.PatientId == patient.PatientId,
            // 🎯 ضفنا اسم الجدول بتاع العطار هنا عشان الـ AutoMapper يعرف يجيب اسمه (عدلها حسب اسم الـ Navigation Property عندك)
            includeProperties: "SubOrders.Herbalist.User",
            tracked: false,
            cancellationToken: cancellationToken);

        if (order == null) return null;

        return _mapper.Map<OrderDetailsResponse>(order);
    }

    // --- 4. المريض بيلغي الطلب ---
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

        // 👈 استخدام الـ Enums لمنع الإلغاء لو بدأ في التجهيز
        if (order.SubOrders.Any(s => s.Status != SubOrderStatus.Pending.ToString()))
            throw new Exception("Cannot cancel order because some items are already being prepared or shipped.");

        order.OrderStatus = OrderStatus.Cancelled.ToString();
        foreach (var subOrder in order.SubOrders)
        {
            subOrder.Status = SubOrderStatus.Cancelled.ToString();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

}


