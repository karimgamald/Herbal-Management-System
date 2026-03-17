using PhytoIntellect.Application.Contracts.Herbs;
using PhytoIntellect.Application.DTOs.HerbDTOs;

public interface IHerbService
{
    // 1️⃣ عرض كل الأعشاب المعتمدة
    Task<IEnumerable<HerbResponse>> GetApprovedHerbsAsync(CancellationToken cancellationToken = default);

    // 2️⃣ عرض تفاصيل عشبة
    Task<HerbResponse?> GetHerbByIdAsync(int herbId,CancellationToken cancellationToken = default);
    // 3 Get the herb with herbalist
    Task<HerbWithHerbalistDto?> GetHerbWithHerbalistAsync(int herbId,CancellationToken cancellationToken = default);

    // 4 اقتراح عشبة جديدة
    Task<HerbResponse?> CreateHerbAsync(int userId,HerbRequest request,CancellationToken cancellationToken = default);

    // 5 تعديل عشبة
    Task<HerbResponse?> UpdateHerbAsync(int herbalistId, int herbId, HerbRequest request, CancellationToken cancellationToken);

    // 6 الموافقة على العشبة (Admin)
    Task<bool> ApproveHerbAsync(int herbId,CancellationToken cancellationToken = default);

    // 7 حذف العشبة
    Task<bool> DeleteHerbAsync(int herbId,CancellationToken cancellationToken = default);
}