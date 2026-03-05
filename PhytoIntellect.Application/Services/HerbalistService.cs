using AutoMapper;
using PhytoIntellect.Application.DTOs.HerbalistDTOs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;

namespace PhytoIntellect.Application.Services;

public class HerbalistService(IUnitOfWork unitOfWork, IMapper mapper) : IHerbalistService
{
    // 1️⃣ العشاب يجيب بروفايله
    public async Task<HerbalistDto?> GetMyProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository
            .GetAsync(h => h.UserId == userId, tracked: false, cancellationToken);

        return herbalist == null ? null : mapper.Map<HerbalistDto>(herbalist);
    }

    // 2️⃣ إنشاء بروفايل
    public async Task<string> CreateProfileAsync(int userId, CreateOrUpdateHerbalistDto request, CancellationToken cancellationToken = default)
    {
        var exists = await unitOfWork.HerbalistRepository
            .GetAsync(h => h.UserId == userId, tracked: false, cancellationToken);

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
    public async Task<string> UpdateMyProfileAsync(int userId, CreateOrUpdateHerbalistDto request, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository
            .GetAsync(h => h.UserId == userId, tracked: true, cancellationToken);

        if (herbalist == null)
            return "Herbalist profile not found.";

        herbalist.Bio = request.Bio;
        herbalist.AvailableFrom = request.AvailableFrom;
        herbalist.AvailableTo = request.AvailableTo;

        unitOfWork.HerbalistRepository.Update(herbalist);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Profile updated successfully.";
    }

    // 4️⃣ جلب عشاب بالـ Id (للإدارة أو العرض)
    public async Task<HerbalistDto?> GetHerbalistByIdAsync(int herbalistId, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository
            .GetAsync(h => h.HerbalistId == herbalistId, tracked: false, cancellationToken);

        return herbalist == null ? null : mapper.Map<HerbalistDto>(herbalist);
    }

    // 5️⃣ عرض كل العشابين
    public async Task<IEnumerable<HerbalistDto>> GetAllHerbalistsAsync(CancellationToken cancellationToken = default)
    {
        var herbalists = await unitOfWork.HerbalistRepository
            .GetAllAsync(tracked: false, cancellationToken: cancellationToken);

        return mapper.Map<IEnumerable<HerbalistDto>>(herbalists);
    }
}