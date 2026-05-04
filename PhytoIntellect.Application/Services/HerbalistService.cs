using AutoMapper;
using AutoMapper.QueryableExtensions;
using PhytoIntellect.Application.Contracts.Herbalists;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
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
    public async Task<PaginatedList<HerbalistResponse>> GetAllHerbalistsAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        // 1. نجيب الـ IQueryable
        var query = unitOfWork.HerbalistRepository.GetQueryable(tracked: false);

        // 2. البحث (Searching)
        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            // تقدر تزود أي عمود تاني زي التخصص (Specialty) مثلاً
            query = query.Where(h => h.User!.FullName.ToLower().Contains(search));
        }

        // 3. الترتيب (Sorting)
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
            query = filters.SortColumn.ToLower() switch
            {
                "fullname" => isDesc ? query.OrderByDescending(h => h.User!.FullName) : query.OrderBy(h => h.User!.FullName),
                // "specialty" => isDesc ? query.OrderByDescending(h => h.Specialty) : query.OrderBy(h => h.Specialty),
                _ => query.OrderByDescending(h => h.HerbalistId) // الديفولت لو بعت عمود غلط
            };
        }
        else
        {
            query = query.OrderByDescending(h => h.HerbalistId); // الديفولت لو مفيش ترتيب مبعوت
        }

        // 4. المابينج باستخدام AutoMapper
        var projectedQuery = query.ProjectTo<HerbalistResponse>(mapper.ConfigurationProvider);

        // 5. تطبيق الـ Pagination
        var paginatedHerbalists = await PaginatedList<HerbalistResponse>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);

        return paginatedHerbalists;
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