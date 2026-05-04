using AutoMapper;
using AutoMapper.QueryableExtensions;
using PhytoIntellect.Application.Contracts.HerbalistAiRecipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Services;

public class HerbalistAiRecipeService(IUnitOfWork unitOfWork, IMapper mapper) : IHerbalistAiRecipeService
{
    public async Task<PaginatedList<HerbalistAiRecipeResponse>> GetMyInventoryAsync(
     int userId,
     RequestFilters filters,
     CancellationToken cancellationToken)
    {
        // 1. نتأكد إن العشاب ده موجود أصلاً
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null) throw new KeyNotFoundException("Herbalist not found.");

        // 2. نجيب الـ IQueryable بتاع الـ Inventory
        var query = unitOfWork.HerbalistAiRecipeRepository.GetQueryable(tracked: false);

        // 3. الفلتر الأساسي: نجيب وصفات العشاب ده بس
        query = query.Where(h => h.HerbalistId == herbalist.HerbalistId);

        // 4. البحث (Searching) - لاحظ إننا بنبحث جوه الـ AiRecipe
        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            // غير Title لـ Name أو للعمود اللي شايل اسم الوصفة عندك
            query = query.Where(h => h.AiRecipe.RecommendedRecipeName.ToLower().Contains(search));
        }

        // 5. الترتيب (Sorting) - برضه هنرتب باسم الوصفة أو بتاريخ إضافتها
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
            query = filters.SortColumn.ToLower() switch
            {
                "recommendedRecipeName" => isDesc ? query.OrderByDescending(h => h.AiRecipe.RecommendedRecipeName) : query.OrderBy(h => h.AiRecipe.RecommendedRecipeName),
                // لو عندك تاريخ مثلاً
                // "date" => isDesc ? query.OrderByDescending(h => h.CreatedAt) : query.OrderBy(h => h.CreatedAt),
                _ => query.OrderByDescending(h => h.AiRecipeId)
            };
        }
        else
        {
            query = query.OrderByDescending(h => h.AiRecipeId);
        }

        // 6. المابينج السحري (مش محتاجين Include لأن ProjectTo بتعملها لوحدها)
        var projectedQuery = query.ProjectTo<HerbalistAiRecipeResponse>(mapper.ConfigurationProvider);

        // 7. الـ Pagination
        var paginatedInventory = await PaginatedList<HerbalistAiRecipeResponse>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);

        return paginatedInventory;
    }

    public async Task<HerbalistAiRecipeResponse?> AddAiRecipeAsync(int userId, AddAiRecipeToInventoryRequest request, CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null) throw new KeyNotFoundException("Herbalist not found.");

        var aiRecipe = await unitOfWork.AiRecipeRepository.GetAsync(
            filter: h => h.Id == request.AiRecipeId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (aiRecipe == null) throw new KeyNotFoundException("This AI recipe does not exist in the system."); // 404

        var existingItem = await unitOfWork.HerbalistAiRecipeRepository.GetAsync(
            filter: h => h.AiRecipeId == request.AiRecipeId && h.HerbalistId == herbalist.HerbalistId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (existingItem != null)
            throw new InvalidOperationException("This AI recipe is already in your inventory. You can update its price instead."); // 400

        var entity = mapper.Map<HerbalistAiRecipe>(request);
        entity.HerbalistId = herbalist.HerbalistId;
        entity.IsActive = true;

        await unitOfWork.HerbalistAiRecipeRepository.CreateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var createdItem = await unitOfWork.HerbalistAiRecipeRepository.GetAsync(
            filter: h => h.AiRecipeId == request.AiRecipeId && h.HerbalistId == herbalist.HerbalistId,
            includeProperties: "AiRecipe",
            cancellationToken: cancellationToken);

        return mapper.Map<HerbalistAiRecipeResponse>(createdItem);
    }

    public async Task<bool> UpdatePriceAsync(int userId, int aiRecipeId, decimal newPrice, CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null) throw new KeyNotFoundException("Herbalist not found.");

        var item = await unitOfWork.HerbalistAiRecipeRepository.GetAsync(
            filter: h => h.AiRecipeId == aiRecipeId && h.HerbalistId == herbalist.HerbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (item == null) throw new KeyNotFoundException("AI recipe not found in your inventory.");

        item.Price = newPrice;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ToggleStatusAsync(int userId, int aiRecipeId, CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null) throw new KeyNotFoundException("Herbalist not found.");

        var item = await unitOfWork.HerbalistAiRecipeRepository.GetAsync(
            filter: h => h.AiRecipeId == aiRecipeId && h.HerbalistId == herbalist.HerbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (item == null) throw new KeyNotFoundException("AI recipe not found in your inventory.");

        item.IsActive = !item.IsActive;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return item.IsActive;
    }

    public async Task<bool> RemoveAiRecipeAsync(int userId, int aiRecipeId, CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null) throw new KeyNotFoundException("Herbalist not found.");

        var item = await unitOfWork.HerbalistAiRecipeRepository.GetAsync(
            filter: h => h.AiRecipeId == aiRecipeId && h.HerbalistId == herbalist.HerbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (item == null) throw new KeyNotFoundException("AI recipe not found in your inventory.");

        unitOfWork.HerbalistAiRecipeRepository.Remove(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<HerbalistWithAiRecipeResponse>> GetHerbalistsByAiRecipeAsync(int aiRecipeId, bool isActive = true, CancellationToken cancellationToken = default)
    {
        var herbalistRecipes = await unitOfWork.HerbalistAiRecipeRepository.GetAllAsync(
            filter: h => h.AiRecipeId == aiRecipeId && h.IsActive == isActive,
            tracked: false,
            includeProperties: "Herbalist.User",
            cancellationToken: cancellationToken);

        return mapper.Map<IEnumerable<HerbalistWithAiRecipeResponse>>(herbalistRecipes);
    }
}