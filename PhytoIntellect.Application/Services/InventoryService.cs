using System;
using System.Collections.Generic;
using System.Text;

using AutoMapper;
using PhytoIntellect.Application.Contracts.Inventory;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;

public class InventoryService(IUnitOfWork unitOfWork, IMapper mapper) : IInventoryService
{
    public async Task<IEnumerable<InventoryResponse>> GetMyInventoryAsync(int userId,CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            throw new Exception("Herbalist not found");

        var herbs = await unitOfWork.HerbalistHerbRepository.GetAllAsync(
            h => h.HerbalistId == herbalist.HerbalistId,
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

    public async Task<InventoryResponse?> AddHerbAsync(
        int userId,
        AddHerbToInventoryRequest request,
        CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            throw new Exception("Herbalist not found");

        var entity = new HerbalistHerb
        {
            HerbId = request.HerbId,
            HerbalistId = herbalist.HerbalistId,
            Price = request.Price,
            IsActive = true
        };

        await unitOfWork.HerbalistHerbRepository.CreateAsync(entity, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var herb = await unitOfWork.HerbRepository.GetAsync(
            h => h.HerbId == request.HerbId,
            tracked: false,
            cancellationToken: cancellationToken);

        return new InventoryResponse
        {
            HerbId = herb.HerbId,
            HerbName = herb.HerbName,
            Price = entity.Price,
            IsActive = entity.IsActive
        };
    }

    public async Task<bool> UpdateInventoryAsync(
        int userId,
        int herbId,
        UpdateInventoryRequest request,
        CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            return false;

        var item = await unitOfWork.HerbalistHerbRepository.GetAsync(
            h => h.HerbId == herbId && h.HerbalistId == herbalist.HerbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (item == null)
            return false;

        item.Price = request.Price;
        item.IsActive = request.IsActive;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> RemoveHerbAsync(
        int userId,
        int herbId,
        CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            return false;

        var item = await unitOfWork.HerbalistHerbRepository.GetAsync(
            h => h.HerbId == herbId && h.HerbalistId == herbalist.HerbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (item == null)
            return false;

        unitOfWork.HerbalistHerbRepository.Remove(item);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
