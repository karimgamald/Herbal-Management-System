using AutoMapper;
using PhytoIntellect.Application.Contracts.Herbalists;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using System.Reflection;

namespace PhytoIntellect.Application.Services;

public class HerbalistService(IUnitOfWork unitOfWork, IMapper mapper) : IHerbalistService
{
    // 1️⃣ العشاب يجيب بروفايله
    public async Task<HerbalistResponse?> GetMyProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository
            .GetAsync(h => h.UserId == userId, tracked: false, cancellationToken: cancellationToken);

        return herbalist == null ? null : mapper.Map<HerbalistResponse>(herbalist);
    }

    // 4️⃣ جلب عشاب بالـ Id (للإدارة أو العرض)
    public async Task<HerbalistResponse?> GetHerbalistByIdAsync(int herbalistId, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository
            .GetAsync(h => h.HerbalistId == herbalistId, tracked: false, cancellationToken: cancellationToken);

        return herbalist == null ? null : mapper.Map<HerbalistResponse>(herbalist);
    }

    // 5️⃣ عرض كل العشابين
    public async Task<IEnumerable<HerbalistResponse>> GetAllHerbalistsAsync(CancellationToken cancellationToken = default)
    {
        var herbalists = await unitOfWork.HerbalistRepository
            .GetAllAsync(tracked: false, cancellationToken: cancellationToken);

        return mapper.Map<IEnumerable<HerbalistResponse>>(herbalists);
    }
    // 2️⃣ إنشاء بروفايل
    public async Task<string> CreateProfileAsync(int userId, CreateOrUpdateHerbalistRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await unitOfWork.HerbalistRepository
            .GetAsync(h => h.UserId == userId, tracked: false, cancellationToken: cancellationToken);

        if (exists != null)
            return "Profile already exists.";

        var herbalist = mapper.Map<Herbalist>(request);
        herbalist.UserId = userId;
        herbalist.AverageRating = 0;

        await unitOfWork.HerbalistRepository.CreateAsync(herbalist, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Herbalist profile created successfully.";
    }

    // 3️⃣ تعديل البروفايل
    public async Task<string> UpdateMyProfileAsync(int userId,CreateOrUpdateHerbalistRequest request,
     CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository
            .GetAsync(h => h.UserId == userId, tracked: true, cancellationToken: cancellationToken);

        if (herbalist == null)
            return "Herbalist profile not found.";

        herbalist.Bio = request.Bio;
        herbalist.AvailableFrom = request.AvailableFrom;
        herbalist.AvailableTo = request.AvailableTo;

        unitOfWork.HerbalistRepository.Update(herbalist);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Profile updated successfully.";
    }
}