using AutoMapper;
using PhytoIntellect.Application.Contracts.Orders;
using PhytoIntellect.Application.Contracts.SubOrders;
using PhytoIntellect.Application.Interfaces;

namespace PhytoIntellect.Application.Services;

public class SubOrderService(IUnitOfWork unitOfWork, IMapper mapper) : ISubOrderService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    // --- 1. يجيب لستة طلبات العطار ---
    public async Task<IEnumerable<SubOrderSummaryResponse>> GetHerbalistSubOrdersAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId)) return Enumerable.Empty<SubOrderSummaryResponse>();

        var herbalist = await _unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == parsedUserId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null) return Enumerable.Empty<SubOrderSummaryResponse>();

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
        if (!int.TryParse(userId, out int parsedUserId)) return null;

        var herbalist = await _unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == parsedUserId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null) return null;

        var subOrder = await _unitOfWork.SubOrderRepository.GetAsync(
            filter: s => s.SubOrderId == subOrderId && s.HerbalistId == herbalist.HerbalistId,
            includeProperties: "OrderRecipes.Recipe,OrderHerbs.Herb",
            tracked: false,
            cancellationToken: cancellationToken);

        if (subOrder == null) return null;

        return new SubOrderDetailsResponse
        {
            SubOrderId = subOrder.SubOrderId,
            SubTotal = subOrder.SubTotal,
            Status = subOrder.Status,
            TrackingNumber = subOrder.TrackingNumber,

            // تعديل الوصفات
            Recipes = subOrder.OrderRecipes.Select(r => new OrderItemResponse
            {
                ItemId = r.RecipeId,
                Name = r.Recipe!.Description ?? "Recipe",
                Quantity = r.Quantity,
                UnitPrice = r.UnitPrice,
                SubTotal = r.SubTotal
            }).ToList(),

            // تعديل الأعشاب
            Herbs = subOrder.OrderHerbs.Select(h => new OrderItemResponse
            {
                ItemId = h.HerbId,
                Name = h.Herb!.HerbName ?? "Herb", // تأكد إن اسم العشبة عندك HerbName ولا Name
                Quantity = h.Quantity,
                UnitPrice = h.UnitPrice,
                SubTotal = h.SubTotal
            }).ToList()
        };
    }

    // --- 3. يغير حالة الطلب (أهم دالة) ---
    public async Task UpdateSubOrderStatusAsync(int subOrderId, string userId, UpdateSubOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out int parsedUserId)) throw new Exception("Invalid User ID.");

        var herbalist = await _unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == parsedUserId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null) throw new Exception("Herbalist not found.");

        var subOrder = await _unitOfWork.SubOrderRepository.GetAsync(
            filter: s => s.SubOrderId == subOrderId && s.HerbalistId == herbalist.HerbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (subOrder == null) throw new Exception("SubOrder not found or access denied.");

        // تحديث الحالة
        subOrder.Status = request.Status;

        // 🎯 اللوجيك الجديد: معالجة رقم التتبع
        // لو العطار بعت رقم تتبع بإيده، احفظه
        if (!string.IsNullOrWhiteSpace(request.TrackingNumber))
        {
            subOrder.TrackingNumber = request.TrackingNumber;
        }
        // لو العطار مبعتش رقم، والحالة اتغيرت لـ Shipped، السيستم هيولد رقم من عنده
        else if (request.Status == "Shipped" && string.IsNullOrWhiteSpace(subOrder.TrackingNumber))
        {
            subOrder.TrackingNumber = GenerateTrackingNumber(herbalist.HerbalistId);
        }

        _unitOfWork.SubOrderRepository.Update(subOrder);

        // فحص هل الأوردر الرئيسي اكتمل ولا لأ
        if (request.Status == "Delivered")
        {
            var mainOrder = await _unitOfWork.OrderRepository.GetAsync(
                filter: o => o.OrderId == subOrder.OrderId,
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

    // --- الدالة المساعدة (لازم تنقلها للـ SubOrderService) ---
    private string GenerateTrackingNumber(int herbalistId)
    {
        string randomString = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        return $"PHYTO-H{herbalistId}-{randomString}";
    }
}