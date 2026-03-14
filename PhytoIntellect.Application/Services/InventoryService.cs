using System;
using AutoMapper;
using PhytoIntellect.Application.Contracts.Inventory;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Services;

public class InventoryService(IUnitOfWork unitOfWork, IMapper mapper) : IInventoryService
{
    public async Task<IEnumerable<InventoryResponse>> GetMyInventoryAsync(int userId, CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            throw new Exception("Herbalist not found.");

        var herbs = await unitOfWork.HerbalistHerbRepository.GetAllAsync(
            filter: h => h.HerbalistId == herbalist.HerbalistId,
            tracked: false,
            includeProperties: "Herb",
            cancellationToken: cancellationToken);

        // هنا الـ Select العادية ممتازة جداً ومش محتاجين AutoMapper
        return herbs.Select(x => new InventoryResponse
        {
            HerbId = x.HerbId,
            HerbName = x.Herb.HerbName,
            Price = x.Price,
            IsActive = x.IsActive
        });
    }

    public async Task<InventoryResponse?> AddHerbAsync(int userId, AddHerbToInventoryRequest request, CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null) throw new Exception("Herbalist not found.");

        // 🚨 حماية 1: هل العشبة دي موجودة أصلاً في قاموس الأعشاب؟
        var herb = await unitOfWork.HerbRepository.GetAsync(
            filter: h => h.HerbId == request.HerbId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herb == null) throw new Exception("This herb does not exist in the system.");

        // 🚨 حماية 2: هل العطار ضاف العشبة دي قبل كده في مخزنه؟
        var existingInventoryItem = await unitOfWork.HerbalistHerbRepository.GetAsync(
            filter: h => h.HerbId == request.HerbId && h.HerbalistId == herbalist.HerbalistId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (existingInventoryItem != null)
            throw new Exception("This herb is already in your inventory. You can update its price instead.");

        var entity = new HerbalistHerb
        {
            HerbId = request.HerbId,
            HerbalistId = herbalist.HerbalistId,
            Price = request.Price,
            IsActive = true
        };

        await unitOfWork.HerbalistHerbRepository.CreateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // بما إننا جبنا العشبة فوق في حماية 1، مش محتاجين نعمل استعلام تاني للداتابيز!
        return new InventoryResponse
        {
            HerbId = herb.HerbId,
            HerbName = herb.HerbName,
            Price = entity.Price,
            IsActive = entity.IsActive
        };
    }

    public async Task<bool> UpdateInventoryAsync(int userId, int herbId, UpdateInventoryRequest request, CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null) return false;

        var item = await unitOfWork.HerbalistHerbRepository.GetAsync(
            filter: h => h.HerbId == herbId && h.HerbalistId == herbalist.HerbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (item == null) return false;

        item.Price = request.Price;
        item.IsActive = request.IsActive;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RemoveHerbAsync(int userId, int herbId, CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null) return false;

        var item = await unitOfWork.HerbalistHerbRepository.GetAsync(
            filter: h => h.HerbId == herbId && h.HerbalistId == herbalist.HerbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (item == null) return false;

        unitOfWork.HerbalistHerbRepository.Remove(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}