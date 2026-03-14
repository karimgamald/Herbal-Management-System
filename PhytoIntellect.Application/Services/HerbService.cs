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
        herb.IsApproved = false;
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