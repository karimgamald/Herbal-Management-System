using AutoMapper;
using AutoMapper.QueryableExtensions;
using PhytoIntellect.Application.Contracts.HerbalistHerb;
using PhytoIntellect.Application.Contracts.Herbs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Services;

public class HerbService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService) : IHerbService
{
    public async Task<PaginatedList<HerbResponse>> GetApprovedHerbsAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.HerbRepository.GetQueryable(tracked: false).Where(h => h.IsApproved);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(h => h.HerbName.ToLower().Contains(search));
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "herbname" => isDesc ? query.OrderByDescending(h => h.HerbName) : query.OrderBy(h => h.HerbName),
                "scientificname" => isDesc ? query.OrderByDescending(h => h.ScientificName) : query.OrderBy(h => h.ScientificName),
                _ => isDesc ? query.OrderByDescending(h => h.HerbName) : query.OrderBy(h => h.HerbName)
            };
        }
        else
        {
            query = isDesc ? query.OrderByDescending(h => h.HerbName) : query.OrderBy(h => h.HerbName);
        }

        var projectedQuery = query.ProjectTo<HerbResponse>(mapper.ConfigurationProvider);
        return await PaginatedList<HerbResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }
    public async Task<PaginatedList<HerbResponse>> GetPendingHerbsAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.HerbRepository.GetQueryable(tracked: false).Where(h => !h.IsApproved);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(h => h.HerbName.ToLower().Contains(search));
        }

        query = query.OrderByDescending(h => h.HerbId);

        var projectedQuery = query.ProjectTo<HerbResponse>(mapper.ConfigurationProvider);
        return await PaginatedList<HerbResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }
    public async Task<HerbResponse?> GetHerbByIdAsync(int herbId, CancellationToken cancellationToken = default)
    {
        var herb = await unitOfWork.HerbRepository.GetAsync(h => h.HerbId == herbId && h.IsApproved, tracked: false, cancellationToken: cancellationToken);
        return herb == null ? null : mapper.Map<HerbResponse>(herb);
    }
    public async Task<HerbWithHerbalistResponse?> GetHerbWithHerbalistAsync(int herbId, CancellationToken cancellationToken = default)
    {
        var herb = await unitOfWork.HerbRepository.GetAsync(h => h.HerbId == herbId && h.IsApproved, includeProperties: "AddedByHerbalist.User", tracked: false, cancellationToken: cancellationToken);
        return herb == null ? null : mapper.Map<HerbWithHerbalistResponse>(herb);
    }
    public async Task<IEnumerable<HerbalistHerbResponse>> GetHerbalistsByHerbIdAsync(int herbId, CancellationToken cancellationToken = default)
    {
        var herbalistHerbs = await unitOfWork.HerbalistHerbRepository.GetAllAsync(hh => hh.HerbId == herbId && hh.IsActive, includeProperties: "Herbalist.User", tracked: false, cancellationToken: cancellationToken);
        var result = herbalistHerbs.Select(hh => new HerbalistHerbResponse
        {
            HerbalistId = hh.HerbalistId,
            HerbalistName = hh.Herbalist?.User?.FullName ?? "Unknown",
            Address = $"{hh.Herbalist?.User?.Governorate}, {hh.Herbalist?.User?.City}, {hh.Herbalist?.User?.Street}",
            Price = hh.Price ?? 0
        });
        return result.OrderBy(x => x.Price);
    }
    public async Task<HerbResponse?> CreateHerbAsync(int userId, HerbRequest request, CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == userId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null) throw new Exception("Herbalist not found.");

        var herb = mapper.Map<Herb>(request);
        herb.IsApproved = false;
        herb.AddedByHerbalistId = herbalist.HerbalistId;

        if (request.Image != null)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "herbs");
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
            using var stream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create);
            await request.Image.CopyToAsync(stream, cancellationToken);
            herb.ImageURL = "/images/herbs/" + fileName;
        }

        await unitOfWork.HerbRepository.CreateAsync(herb, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return mapper.Map<HerbResponse>(herb);
    }
    public async Task<HerbResponse?> UpdateHerbAsync(int userId, int herbId, HerbRequest request, CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == userId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null) throw new Exception("Herbalist not found.");

        var herb = await unitOfWork.HerbRepository.GetAsync(h => h.HerbId == herbId && h.AddedByHerbalistId == herbalist.HerbalistId, tracked: true, cancellationToken: cancellationToken);

        if (herb == null) return null;
        if (herb.IsApproved) throw new Exception("Cannot update an approved global herb.");

        mapper.Map(request, herb);

        if (request.Image != null)
        {
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "herbs");
            if (!string.IsNullOrEmpty(herb.ImageURL))
            {
                var oldFilePath = Path.Combine(uploadPath, Path.GetFileName(herb.ImageURL));
                if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
            }
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName);
            using var stream = new FileStream(Path.Combine(uploadPath, newFileName), FileMode.Create);
            await request.Image.CopyToAsync(stream, cancellationToken);
            herb.ImageURL = "/images/herbs/" + newFileName;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return mapper.Map<HerbResponse>(herb);
    }
    public async Task<bool> DeleteHerbAsync(int userId, int herbId, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == userId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null) return false;

        var herb = await unitOfWork.HerbRepository.GetAsync(h => h.HerbId == herbId && h.AddedByHerbalistId == herbalist.HerbalistId, tracked: true, cancellationToken: cancellationToken);

        if (herb == null) return false;
        if (herb.IsApproved) throw new Exception("Cannot delete an approved global herb.");

        if (!string.IsNullOrEmpty(herb.ImageURL))
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", herb.ImageURL.TrimStart('/'));
            if (File.Exists(imagePath)) File.Delete(imagePath);
        }

        unitOfWork.HerbRepository.Remove(herb);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task<HerbResponse> AdminCreateHerbAsync(HerbRequest request, CancellationToken cancellationToken = default)
    {
        var herb = mapper.Map<Herb>(request);
        herb.IsApproved = true;
        herb.AddedByHerbalistId = null;

        if (request.Image != null)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "herbs");
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
            using var stream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create);
            await request.Image.CopyToAsync(stream, cancellationToken);
            herb.ImageURL = "/images/herbs/" + fileName;
        }

        await unitOfWork.HerbRepository.CreateAsync(herb, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return mapper.Map<HerbResponse>(herb);
    }
    public async Task<HerbResponse?> AdminUpdateHerbAsync(int herbId, HerbRequest request, CancellationToken cancellationToken = default)
    {
        var herb = await unitOfWork.HerbRepository.GetAsync(h => h.HerbId == herbId, tracked: true, cancellationToken: cancellationToken);
        if (herb == null) return null;

        mapper.Map(request, herb);

        if (request.Image != null)
        {
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "herbs");
            if (!string.IsNullOrEmpty(herb.ImageURL))
            {
                var oldFilePath = Path.Combine(uploadPath, Path.GetFileName(herb.ImageURL));
                if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
            }
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName);
            using var stream = new FileStream(Path.Combine(uploadPath, newFileName), FileMode.Create);
            await request.Image.CopyToAsync(stream, cancellationToken);
            herb.ImageURL = "/images/herbs/" + newFileName;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return mapper.Map<HerbResponse>(herb);
    }
    public async Task<bool> AdminDeleteHerbAsync(int herbId, CancellationToken cancellationToken = default)
    {
        var herb = await unitOfWork.HerbRepository.GetAsync(h => h.HerbId == herbId, tracked: true, cancellationToken: cancellationToken);
        if (herb == null) return false;

        if (!string.IsNullOrEmpty(herb.ImageURL))
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", herb.ImageURL.TrimStart('/'));
            if (File.Exists(imagePath)) File.Delete(imagePath);
        }

        if (herb.AddedByHerbalistId.HasValue)
        {
            var herbalist = await unitOfWork.HerbalistRepository.GetAsync(h => h.HerbalistId == herb.AddedByHerbalistId.Value, tracked: false, cancellationToken: cancellationToken);
            if (herbalist != null)
            {
                await notificationService.SendNotificationAsync(
                    userId: herbalist.UserId,
                    title: "Herb Rejected/Removed ❌",
                    message: $"System Notice: The herb '{herb.HerbName}' has been removed from the system by the administration.",
                    cancellationToken: cancellationToken);
            }
        }

        unitOfWork.HerbRepository.Remove(herb);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task<bool> ApproveHerbAsync(int herbId, CancellationToken cancellationToken = default)
    {
        var herb = await unitOfWork.HerbRepository.GetAsync(h => h.HerbId == herbId, tracked: true, cancellationToken: cancellationToken);
        if (herb == null) return false;

        herb.IsApproved = true;

        if (herb.AddedByHerbalistId.HasValue)
        {
            var herbalist = await unitOfWork.HerbalistRepository.GetAsync(h => h.HerbalistId == herb.AddedByHerbalistId.Value, tracked: false, cancellationToken: cancellationToken);
            if (herbalist != null)
            {
                await notificationService.SendNotificationAsync(
                    userId: herbalist.UserId,
                    title: "Herb Approved! 🎉",
                    message: $"Good news! Your proposed herb '{herb.HerbName}' has been approved and is now live in the global catalog.",
                    cancellationToken: cancellationToken);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

}