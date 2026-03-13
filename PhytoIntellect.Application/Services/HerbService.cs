using AutoMapper;
using PhytoIntellect.Application.Contracts.Herbs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Services;

public class HerbService(IUnitOfWork unitOfWork, IMapper mapper) : IHerbService
{
    // 1️⃣ Get All Approved Herbs
    public async Task<IEnumerable<HerbResponse>> GetApprovedHerbsAsync(
        CancellationToken cancellationToken = default)
    {
        var herbs = await unitOfWork.HerbRepository.GetAllAsync(
            filter: h => h.IsApproved,
            tracked: false,
            cancellationToken: cancellationToken);

        return mapper.Map<IEnumerable<HerbResponse>>(herbs);
    }

    // 2️⃣ Get Herb By Id
    public async Task<HerbResponse?> GetHerbByIdAsync(
        int herbId,
        CancellationToken cancellationToken = default)
    {
        var herb = await unitOfWork.HerbRepository.GetAsync(
            filter: h => h.HerbId == herbId && h.IsApproved,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herb == null)
            return null;

        return mapper.Map<HerbResponse>(herb);
    }

    // 3️⃣ Create Herb
    // 3️⃣ اقتراح عشبة جديدة
    public async Task<HerbResponse?> CreateHerbAsync(int herbalistId, HerbRequest request, CancellationToken cancellationToken)
    {
        // 🔹 التأكد إن العطار موجود
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            h => h.HerbalistId == herbalistId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            throw new Exception("Herbalist not found.");

        var herb = mapper.Map<Herb>(request);
        herb.IsApproved = false;
        herb.AddedByHerbalistId = herbalistId;

        // 📷 رفع الصورة
        if (request.Image != null)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/herbs");

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
    public async Task<HerbResponse?> UpdateHerbAsync(int herbalistId, int herbId, HerbRequest request, CancellationToken cancellationToken)
    {
        // 🔹 التأكد من العطار موجود
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            h => h.HerbalistId == herbalistId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            throw new Exception("Herbalist not found.");

        var herb = await unitOfWork.HerbRepository.GetAsync(
            h => h.HerbId == herbId && h.AddedByHerbalistId == herbalistId, // ✅ فقط عشبة هذا العطار
            tracked: true,
            cancellationToken: cancellationToken);

        if (herb == null)
            return null;

        mapper.Map(request, herb);

        // 📷 تحديث الصورة إذا تم رفعها
        if (request.Image != null)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/herbs");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await request.Image.CopyToAsync(stream, cancellationToken);

            herb.ImageURL = "/images/herbs/" + fileName;
        }

        unitOfWork.HerbRepository.Update(herb);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<HerbResponse>(herb);
    }
    // 5️⃣ Approve Herb
    public async Task<bool> ApproveHerbAsync(
        int herbId,
        CancellationToken cancellationToken = default)
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
    public async Task<bool> DeleteHerbAsync(
        int herbId,
        CancellationToken cancellationToken = default)
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