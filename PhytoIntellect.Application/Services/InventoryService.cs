using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            throw new KeyNotFoundException("Herbalist not found."); // 👈 تعديل

        var herbs = await unitOfWork.HerbalistHerbRepository.GetAllAsync(
            filter: h => h.HerbalistId == herbalist.HerbalistId,
            tracked: false,
            includeProperties: "Herb",
            cancellationToken: cancellationToken);

        return herbs.Select(x => new InventoryResponse
        {
            HerbId = x.HerbId,
            HerbName = x.Herb.HerbName,
            Price = x.Price,
            IsActive = x.IsActive
        });
    }

    public async Task<IEnumerable<InventoryResponse>> GetAllByHerbalistIdAsync(int herbalistId, CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.HerbalistId == herbalistId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            throw new KeyNotFoundException("Herbalist not found."); // 👈 تعديل

        var herbs = await unitOfWork.HerbalistHerbRepository.GetAllAsync(
            filter: h => h.HerbalistId == herbalist.HerbalistId,
            tracked: false,
            includeProperties: "Herb",
            cancellationToken: cancellationToken);

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

        if (herbalist == null)
            throw new KeyNotFoundException("Herbalist not found.");

        var herb = await unitOfWork.HerbRepository.GetAsync(
            filter: h => h.HerbId == request.HerbId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herb == null)
            throw new KeyNotFoundException("This herb does not exist in the system."); // 👈 404

        var existingInventoryItem = await unitOfWork.HerbalistHerbRepository.GetAsync(
            filter: h => h.HerbId == request.HerbId && h.HerbalistId == herbalist.HerbalistId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (existingInventoryItem != null)
            throw new InvalidOperationException("This herb is already in your inventory. You can update its price instead."); // 👈 400

        var entity = new HerbalistHerb
        {
            HerbId = request.HerbId,
            HerbalistId = herbalist.HerbalistId,
            Price = request.Price,
            IsActive = true
        };

        await unitOfWork.HerbalistHerbRepository.CreateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new InventoryResponse
        {
            HerbId = herb.HerbId,
            HerbName = herb.HerbName,
            Price = entity.Price,
            IsActive = entity.IsActive
        };
    }

    // 🎯 شيلنا الـ return false وخليناها ترمي Exception عشان الـ Controller يفهم المشكلة فين
    public async Task<bool> UpdateInventoryAsync(int userId, int herbId, UpdateInventoryRequest request, CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null) throw new KeyNotFoundException("Herbalist not found.");

        var item = await unitOfWork.HerbalistHerbRepository.GetAsync(
            filter: h => h.HerbId == herbId && h.HerbalistId == herbalist.HerbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (item == null) throw new KeyNotFoundException("Herb not found in your inventory.");

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

        if (herbalist == null) throw new KeyNotFoundException("Herbalist not found.");

        var item = await unitOfWork.HerbalistHerbRepository.GetAsync(
            filter: h => h.HerbId == herbId && h.HerbalistId == herbalist.HerbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (item == null) throw new KeyNotFoundException("Herb not found in your inventory.");

        unitOfWork.HerbalistHerbRepository.Remove(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}