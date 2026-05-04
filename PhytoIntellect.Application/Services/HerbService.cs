using AutoMapper;
using AutoMapper.QueryableExtensions;
using PhytoIntellect.Application.Contracts.HerbalistHerb;
using PhytoIntellect.Application.Contracts.Herbs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Services;

public class HerbService(IUnitOfWork unitOfWork, IMapper mapper) : IHerbService
{
    // 1️⃣ Get All Approved Herbs
    public async Task<PaginatedList<HerbResponse>> GetApprovedHerbsAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        // 1. نجيب الـ IQueryable
        var query = unitOfWork.HerbRepository.GetQueryable(tracked: false);

        // 2. الفلتر الأساسي (الأعشاب المتوافق عليها بس)
        // ده بيعوض الـ filter اللي كان مبعوت في الـ GetAllAsync القديمة
        query = query.Where(h => h.IsApproved);

        // 3. البحث (Searching)
        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            // تقدر تزود هنا أي عواميد تانية حابب اليوزر يبحث فيها زي ScientificName مثلاً
            query = query.Where(h => h.HerbName.ToLower().Contains(search) ||
                                     h.Description!.ToLower().Contains(search));
        }

        // 4. الترتيب (Sorting)
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
            query = filters.SortColumn.ToLower() switch
            {
                "herbName" => isDesc ? query.OrderByDescending(h => h.HerbName) : query.OrderBy(h => h.HerbName),
                // لو عندك تاريخ إضافة العشبة مثلاً:
                // "date" => isDesc ? query.OrderByDescending(h => h.CreatedAt) : query.OrderBy(h => h.CreatedAt),
                _ => query.OrderByDescending(h => h.HerbId) // الديفولت لو بعت عمود غلط
            };
        }
        else
        {
            query = query.OrderByDescending(h => h.HerbId); // الديفولت لو مفيش ترتيب مبعوت
        }

        // 5. المابينج باستخدام AutoMapper (تأكد إنك عامل using AutoMapper.QueryableExtensions;)
        var projectedQuery = query.ProjectTo<HerbResponse>(mapper.ConfigurationProvider);

        // 6. تطبيق الـ Pagination الجاهز
        var paginatedHerbs = await PaginatedList<HerbResponse>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);

        return paginatedHerbs;
    }
    // 2️⃣ Get Herb By Id
    public async Task<HerbResponse?> GetHerbByIdAsync(int herbId,CancellationToken cancellationToken = default)
    {
        var herb = await unitOfWork.HerbRepository.GetAsync(
            filter: h => h.HerbId == herbId && h.IsApproved,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herb == null)
            return null;

        return mapper.Map<HerbResponse>(herb);
    }
    // 3 Get the herb with herbalist
    public async Task<HerbWithHerbalistResponse?> GetHerbWithHerbalistAsync(int herbId,
            CancellationToken cancellationToken = default)
    {
        var herb = await unitOfWork.HerbRepository.GetAsync(
            filter: h => h.HerbId == herbId && h.IsApproved,
            includeProperties: "AddedByHerbalist.User", // لازم يكون الاسم مظبوط
            tracked: false,
            cancellationToken: cancellationToken);

        if (herb == null)
            return null;

        return mapper.Map<HerbWithHerbalistResponse>(herb);
    }

    // Get herbalist (id-name-address) that added this herb to their inventories by herbid
    public async Task<IEnumerable<HerbalistHerbResponse>> GetHerbalistsByHerbIdAsync(int herbId,CancellationToken cancellationToken = default)
    {
        var herbalistHerbs = await unitOfWork.HerbalistHerbRepository.GetAllAsync(
            filter: hh => hh.HerbId == herbId && hh.IsActive,
            includeProperties: "Herbalist.User",
            tracked: false,
            cancellationToken: cancellationToken);

        var result = herbalistHerbs.Select(hh => new HerbalistHerbResponse
        {
            HerbalistId = hh.HerbalistId,
            HerbalistName = hh.Herbalist?.User?.FullName ?? "Unknown",
            Address = $"{hh.Herbalist?.User?.Governorate}, {hh.Herbalist?.User?.City}, {hh.Herbalist?.User?.Street}",
            Price = hh.Price ?? 0
        });

        // ترتيب حسب السعر =>الأرخص الأول
        return result.OrderBy(x => x.Price);
    }
    public async Task<HerbResponse?> CreateHerbAsync(int userId, HerbRequest request, CancellationToken cancellationToken)
    {
        // 👈 بندور بالـ UserId اللي جاي من التوكن
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            throw new Exception("Herbalist not found.");

        var herb = mapper.Map<Herb>(request);
        herb.IsApproved = true;
        herb.AddedByHerbalistId = herbalist.HerbalistId; // بنربط العشبة بـ ID العطار 

        // 📷 رفع الصورة
        if (request.Image != null)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "herbs");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await request.Image.CopyToAsync(stream, cancellationToken);

            herb.ImageURL = "/images/herbs/" + fileName;
        }

        await unitOfWork.HerbRepository.CreateAsync(herb, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<HerbResponse>(herb);
    }

    // 4️⃣ تعديل عشبة
    public async Task<HerbResponse?> UpdateHerbAsync(int userId, int herbId, HerbRequest request, CancellationToken cancellationToken)
    {
        // 👈 بندور بالـ UserId
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            throw new Exception("Herbalist not found.");

        var herb = await unitOfWork.HerbRepository.GetAsync(
            h => h.HerbId == herbId && h.AddedByHerbalistId == herbalist.HerbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (herb == null)
            return null; // لو العشبة مش موجودة أو العطار ده مش هو صاحبها

        // بنعمل Map للبيانات النصية
        mapper.Map(request, herb);

        // 📷 تحديث الصورة إذا تم رفعها
        if (request.Image != null)
        {
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "herbs");

            if (!string.IsNullOrEmpty(herb.ImageURL))
            {
                var oldFileName = Path.GetFileName(herb.ImageURL);
                var oldFilePath = Path.Combine(uploadPath, oldFileName);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName);
            var newFilePath = Path.Combine(uploadPath, newFileName);

            using var stream = new FileStream(newFilePath, FileMode.Create);
            await request.Image.CopyToAsync(stream, cancellationToken);

            herb.ImageURL = "/images/herbs/" + newFileName;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<HerbResponse>(herb);
    }


    // 5️⃣ Approve Herb
    public async Task<bool> ApproveHerbAsync(int herbId,CancellationToken cancellationToken = default)
    {
        var herb = await unitOfWork.HerbRepository.GetAsync(
            filter: h => h.HerbId == herbId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (herb == null)
            return false;

        herb.IsApproved = true;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    // 6️⃣ Delete Herb
    public async Task<bool> DeleteHerbAsync(int herbId,CancellationToken cancellationToken = default)
    {
        var herb = await unitOfWork.HerbRepository.GetAsync(
            filter: h => h.HerbId == herbId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (herb == null)
            return false;

        // حذف الصورة
        if (!string.IsNullOrEmpty(herb.ImageURL))
        {
            var imagePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                herb.ImageURL.TrimStart('/'));

            if (File.Exists(imagePath))
                File.Delete(imagePath);
        }

        unitOfWork.HerbRepository.Remove(herb);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}