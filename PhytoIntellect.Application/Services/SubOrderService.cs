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

    // --- 1. يجيب لستة طلبات العطار ---
    public async Task<IEnumerable<SubOrderSummaryResponse>> GetHerbalistSubOrdersAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId)) 
            return Enumerable.Empty<SubOrderSummaryResponse>();

        var herbalist = await _unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == parsedUserId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null) 
            return Enumerable.Empty<SubOrderSummaryResponse>();

        var subOrders = await _unitOfWork.SubOrderRepository.GetAllAsync(
            filter: s => s.HerbalistId == herbalist.HerbalistId,
            // 🎯 ضفنا الـ Include ده عشان الـ AutoMapper يعرف يجيب اسم العطار لو طلبناه في الـ DTO
            includeProperties: "Herbalist.User",
            tracked: false,
            cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<SubOrderSummaryResponse>>(subOrders);
    }

    // --- 2. يجيب تفاصيل طلب معين عشان يجهزه ---
    public async Task<SubOrderDetailsResponse?> GetSubOrderDetailsAsync(int subOrderId, string userId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId)) 
            return null;

        var herbalist = await _unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == parsedUserId, tracked: false, cancellationToken: cancellationToken);

        if (herbalist == null) 
            return null;

        var subOrder = await _unitOfWork.SubOrderRepository.GetAsync(
            filter: s => s.SubOrderId == subOrderId && s.HerbalistId == herbalist.HerbalistId,
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
            TrackingNumber = subOrder.ExternalDeliveryID,

            // تعديل الوصفات
            Recipes = subOrder.OrderRecipes.Select(r => new OrderRecipeResponse//OrderItemResponse
            {
                RecipeId = r.RecipeId,
                RecipeName = r.Recipe!.Description ?? "Recipe",
                QuantityPerOne = r.Quantity,
                UnitPricePerOne = r.UnitPrice,
                SubTotal = r.SubTotal,
            }).ToList(),

            // تعديل الأعشاب
            Herbs = subOrder.OrderHerbs.Select(h => new OrderHerbResponse//OrderItemResponse
            {
                HerbId = h.HerbId,
                HerbName = h.Herb!.HerbName ?? "Herb", // تأكد إن اسم العشبة عندك HerbName ولا Name
                QuantityPerGram = h.Quantity,
                UnitPricePerKilo = h.UnitPrice,
                SubTotal = h.SubTotal
            }).ToList()
        };
    }

    // --- 3. يغير حالة الطلب (أهم دالة) ---
    public async Task UpdateSubOrderStatusAsync(int subOrderId, string userId, UpdateSubOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        // 1. لو الـ ID بايظ 👈 نرمي ArgumentException (عشان دي مشكلة في الداتا اللي مبعوتة)
        if (!int.TryParse(userId, out int parsedUserId))
            throw new ArgumentException("Invalid User ID format.");

        // 2. لو العطار مش موجود 👈 نرمي UnauthorizedAccessException (عشان ده يوزر بيحاول يكسر الصلاحيات)
        var herbalist = await _unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == parsedUserId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null)
            throw new UnauthorizedAccessException("Herbalist account not found or access denied.");

        // 3. لو الحالة مش في الـ Enum 👈 نرمي ArgumentException (عشان برضه الداتا المبعوتة غلط)
        if (!Enum.TryParse<SubOrderStatus>(request.Status, true, out var newSubStatus))
            throw new ArgumentException("Invalid SubOrder Status. Please provide a valid status like Preparing or Shipped.");

        // 4. لو الأوردر مش بتاعه أو مش موجود 👈 نرمي KeyNotFoundException (زي ما ظبطناها سوا)
        var subOrder = await _unitOfWork.SubOrderRepository.GetAsync(
            filter: s => s.SubOrderId == subOrderId && s.HerbalistId == herbalist.HerbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (subOrder == null)
            throw new KeyNotFoundException("SubOrder not found or you do not have permission to access it.");

        // --- بقية اللوجيك بتاعك سليم وزي الفل ---
        subOrder.Status = newSubStatus.ToString();

        // معالجة رقم التتبع
        if (newSubStatus == SubOrderStatus.Shipped && string.IsNullOrWhiteSpace(subOrder.ExternalDeliveryID))
        {
            subOrder.ExternalDeliveryID = GenerateTrackingNumber(herbalist.HerbalistId);
        }

        _unitOfWork.SubOrderRepository.Update(subOrder);

        // 🎯 اللوجيك الذكي لتحديث الأوردر الرئيسي
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

    // 🧠 دالة الذكاء الاصطناعي (بتفهم البيزنس وتحدد حالة الأوردر الكبير)
    private string DetermineMainOrderStatus(List<string> subStatuses)
    {
        // 1. لو كلهم اتلغوا 👈 الأوردر الرئيسي اتلغى
        if (subStatuses.All(s => s == SubOrderStatus.Cancelled.ToString()))
            return OrderStatus.Cancelled.ToString();

        // 2. لو كلهم اتوصلوا (أو ميكس بين اتوصل واتلغى) 👈 تم التوصيل بنجاح
        if (subStatuses.All(s => s == SubOrderStatus.Delivered.ToString() || s == SubOrderStatus.Cancelled.ToString()))
        {
            if (subStatuses.Contains(SubOrderStatus.Cancelled.ToString()))
                return OrderStatus.PartiallyDelivered.ToString();

            return OrderStatus.Delivered.ToString();
        }

        // 3. لو كلهم اتشحنوا (أو جزء اتلغى وجزء اتشحن) 👈 تم الشحن
        if (subStatuses.All(s => s == SubOrderStatus.Shipped.ToString() || s == SubOrderStatus.Delivered.ToString() || s == SubOrderStatus.Cancelled.ToString()))
        {
            if (subStatuses.Contains(SubOrderStatus.Cancelled.ToString()))
                return OrderStatus.PartiallyShipped.ToString();

            return OrderStatus.Shipped.ToString();
        }

        // 4. لو أي عطار بدأ يجهز 👈 جاري المعالجة (التعديل هنا: بقينا بنسأل على Preparing)
        if (subStatuses.Any(s => s == SubOrderStatus.Preparing.ToString() || s == SubOrderStatus.Shipped.ToString()))
            return OrderStatus.Processing.ToString();

        // 5. في أي حالة تانية (زي إنهم كلهم لسه Pending)
        return OrderStatus.Pending.ToString();
    }
}