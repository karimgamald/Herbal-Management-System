using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PhytoIntellect.Application.Contracts.Inventory;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces; // تأكد من وجود الـ Interface للـ UnitOfWork
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Application.Services;

public class InventoryHerbsService(IUnitOfWork unitOfWork, IMapper mapper) : IInventoryHerbsService
{
    public async Task<PaginatedList<InventoryResponse>> GetMyInventoryAsync(
        int userId,
        RequestFilters filters,
        CancellationToken cancellationToken = default)
    {
        // 1. نتأكد إن العشاب موجود
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            throw new KeyNotFoundException("Herbalist not found.");

        // 2. نجيب الـ IQueryable (مخزون العشاب ده بس)
        var query = unitOfWork.HerbalistHerbRepository.GetQueryable(tracked: false)
            .Where(h => h.HerbalistId == herbalist.HerbalistId);

        // 3. البحث (هيبحث في اسم العشبة اللي جوه جدول الـ Herb)
        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(h => h.Herb.HerbName.ToLower().Contains(search));
        }

        // 4. الترتيب (رتبنا بالاسم أو السعر)
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
            query = filters.SortColumn.ToLower() switch
            {
                "name" => isDesc ? query.OrderByDescending(h => h.Herb.HerbName) : query.OrderBy(h => h.Herb.HerbName),
                "price" => isDesc ? query.OrderByDescending(h => h.Price) : query.OrderBy(h => h.Price),
                _ => isDesc ? query.OrderByDescending(h => h.HerbId) : query.OrderBy(h => h.HerbId)
            };
        }
        else
        {
            query = query.OrderByDescending(h => h.HerbId);
        }

        // 5. تم التحويل إلى ProjectTo لتوحيد الأداء والنقاء البرمجي
        var projectedQuery = query.ProjectTo<InventoryResponse>(mapper.ConfigurationProvider);

        // 6. تطبيق الـ Pagination
        return await PaginatedList<InventoryResponse>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);
    }

    public async Task<PaginatedList<InventoryResponse>> GetAllByHerbalistIdAsync(
        int herbalistId,
        RequestFilters filters,
        CancellationToken cancellationToken = default)
    {
        var herbalistExists = await unitOfWork.HerbalistRepository.GetAsync(
            h => h.HerbalistId == herbalistId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalistExists == null)
            return new PaginatedList<InventoryResponse>(
                new List<InventoryResponse>(),
                0,
                filters.PageNumber,
                filters.PageSize);

        var query = unitOfWork.HerbalistHerbRepository
            .GetQueryable(tracked: false)
            .Where(x => x.HerbalistId == herbalistId);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(x => x.Herb.HerbName.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = filters.SortDirection?.ToUpper() == "DESC";

            query = filters.SortColumn.ToLower() switch
            {
                "name" => isDesc ? query.OrderByDescending(x => x.Herb.HerbName) : query.OrderBy(x => x.Herb.HerbName),
                "price" => isDesc ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price),
                _ => isDesc ? query.OrderByDescending(x => x.HerbId) : query.OrderBy(x => x.HerbId)
            };
        }
        else
        {
            query = query.OrderBy(x => x.HerbId);
        }

        var projectedQuery = query.ProjectTo<InventoryResponse>(mapper.ConfigurationProvider);

        return await PaginatedList<InventoryResponse>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);
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
            throw new KeyNotFoundException("This herb does not exist in the system.");

        var existingInventoryItem = await unitOfWork.HerbalistHerbRepository.GetAsync(
            filter: h => h.HerbId == request.HerbId && h.HerbalistId == herbalist.HerbalistId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (existingInventoryItem != null)
            throw new InvalidOperationException("This herb is already in your inventory. You can update its price instead.");

        var entity = new HerbalistHerb
        {
            HerbId = request.HerbId,
            HerbalistId = herbalist.HerbalistId,
            Price = request.PricePerKilo,
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

        item.Price = request.PricePerKilo;
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

    // ================= Admin Services =================

    public async Task<PaginatedList<InventoryResponse>> GetAllInventoryByAdminAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        // تم الإصلاح: الاستعلام يجب أن يبدأ من الـ HerbalistHerbRepository لرؤية المخزون المشترك
        var query = unitOfWork.HerbalistHerbRepository.GetQueryable(tracked: false);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(h =>
                h.Herb.HerbName.ToLower().Contains(search) ||
                h.Herbalist.User!.FullName.ToLower().Contains(search));
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "herbname" => isDesc ? query.OrderByDescending(h => h.Herb.HerbName) : query.OrderBy(h => h.Herb.HerbName),
                "price" => isDesc ? query.OrderByDescending(h => h.Price) : query.OrderBy(h => h.Price), // تم تعديلها من quantity إلى price بناءً على جدول الأعشاب
                "herbalistname" => isDesc ? query.OrderByDescending(h => h.Herbalist.User!.FullName) : query.OrderBy(h => h.Herbalist.User!.FullName),
                _ => isDesc ? query.OrderByDescending(h => h.Herb.HerbName) : query.OrderBy(h => h.Herb.HerbName)
            };
        }
        else
        {
            query = isDesc ? query.OrderByDescending(h => h.Herb.HerbName) : query.OrderBy(h => h.Herb.HerbName);
        }

        var projectedQuery = query.ProjectTo<InventoryResponse>(mapper.ConfigurationProvider);
        return await PaginatedList<InventoryResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }

    public async Task<bool> RemoveHerbByAdminAsync(int herbalistId, int herbId, CancellationToken cancellationToken = default)
    {
        // تم الإصلاح: الحذف يتم من مستودع الـ HerbalistHerbRepository المشترك
        var item = await unitOfWork.HerbalistHerbRepository.GetAsync(
            filter: h => h.HerbId == herbId && h.HerbalistId == herbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (item == null) return false;

        unitOfWork.HerbalistHerbRepository.Remove(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}