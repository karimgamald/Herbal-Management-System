using PhytoIntellect.Application.Contracts.Herbs;

public interface IHerbService
{
    // 1️⃣ عرض كل الأعشاب المعتمدة
    Task<IEnumerable<HerbResponse>> GetApprovedHerbsAsync(CancellationToken cancellationToken = default);

    // 2️⃣ عرض تفاصيل عشبة
    Task<HerbResponse?> GetHerbByIdAsync(int herbId,CancellationToken cancellationToken = default);

    // 3️⃣ اقتراح عشبة جديدة
    Task<HerbResponse?> CreateHerbAsync(int userId,HerbRequest request,CancellationToken cancellationToken = default);

    // 4️⃣ تعديل عشبة
    Task<HerbResponse?> UpdateHerbAsync(int herbalistId, int herbId, HerbRequest request, CancellationToken cancellationToken);

    // 5️⃣ الموافقة على العشبة (Admin)
    Task<bool> ApproveHerbAsync(int herbId,CancellationToken cancellationToken = default);

    // 6️⃣ حذف العشبة
    Task<bool> DeleteHerbAsync(int herbId,CancellationToken cancellationToken = default);
}