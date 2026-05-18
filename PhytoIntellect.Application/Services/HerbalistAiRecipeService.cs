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

public class HerbalistAiRecipeService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService) : IHerbalistAiRecipeService
{
    public async Task<PaginatedList<HerbalistAiRecipeResponse>> GetMyInventoryAsync(int userId, RequestFilters filters,
     CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null) throw new KeyNotFoundException("Herbalist not found.");

        var query = unitOfWork.HerbalistAiRecipeRepository.GetQueryable(tracked: false);

        query = query.Where(h => h.HerbalistId == herbalist.HerbalistId);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(h => h.AiRecipe.RecommendedRecipeName.ToLower().Contains(search));
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "recommendedrecipename" => isDesc ? query.OrderByDescending(h => h.AiRecipe.RecommendedRecipeName) : query.OrderBy(h => h.AiRecipe.RecommendedRecipeName),
                "price" => isDesc ? query.OrderByDescending(h => h.Price) : query.OrderBy(h => h.Price),
                _ => isDesc ? query.OrderByDescending(h => h.AiRecipe.RecommendedRecipeName) : query.OrderBy(h => h.AiRecipe.RecommendedRecipeName)
            };
        }
        else
        {
            query = isDesc ? query.OrderByDescending(h => h.AiRecipe.RecommendedRecipeName) : query.OrderBy(h => h.AiRecipe.RecommendedRecipeName);
        }

        var projectedQuery = query.ProjectTo<HerbalistAiRecipeResponse>(mapper.ConfigurationProvider);

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
            includeProperties: "User", 
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null) throw new KeyNotFoundException("Herbalist not found.");

        var aiRecipe = await unitOfWork.AiRecipeRepository.GetAsync(
            filter: h => h.Id == request.AiRecipeId,
            includeProperties: "Patient", 
            tracked: true,
            cancellationToken: cancellationToken);

        if (aiRecipe == null) throw new KeyNotFoundException("This AI recipe does not exist in the system.");

        var existingItem = await unitOfWork.HerbalistAiRecipeRepository.GetAsync(
            filter: h => h.AiRecipeId == request.AiRecipeId && h.HerbalistId == herbalist.HerbalistId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (existingItem != null)
            throw new InvalidOperationException("This AI recipe is already in your inventory. You can update its price instead.");

        var entity = mapper.Map<HerbalistAiRecipe>(request);
        entity.HerbalistId = herbalist.HerbalistId;
        entity.IsActive = true;

        await unitOfWork.HerbalistAiRecipeRepository.CreateAsync(entity, cancellationToken);

        aiRecipe.IsAvailable = true;

        if (aiRecipe.Patient != null)
        {
            await notificationService.SendNotificationAsync(
                userId: aiRecipe.Patient.UserId,
                title: "Recipe Now Available! ✨",
                message: $"Herbalist {herbalist.User?.FullName} has added your AI Recipe '{aiRecipe.RecommendedRecipeName}' to their inventory for {request.Price} EGP. You can now order it!",
                cancellationToken: cancellationToken);
        }

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

        if (!item.IsActive)
        {
            await CheckAndUpdateAvailability(aiRecipeId, herbalist.HerbalistId, cancellationToken);
        }
        else
        {
            var recipe = await unitOfWork.AiRecipeRepository.GetAsync(r => r.Id == aiRecipeId, tracked: true, cancellationToken: cancellationToken);
            if (recipe != null) recipe.IsAvailable = true;
        }

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

        await CheckAndUpdateAvailability(aiRecipeId, herbalist.HerbalistId, cancellationToken);

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

    private async Task CheckAndUpdateAvailability(int aiRecipeId, int excludedHerbalistId, CancellationToken cancellationToken)
    {
        var anyOtherActive = await unitOfWork.HerbalistAiRecipeRepository.GetAsync(
            filter: h => h.AiRecipeId == aiRecipeId && h.IsActive && h.HerbalistId != excludedHerbalistId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (anyOtherActive == null)
        {
            var recipeToUpdate = await unitOfWork.AiRecipeRepository.GetAsync(
                filter: r => r.Id == aiRecipeId,
                tracked: true,
                cancellationToken: cancellationToken);

            if (recipeToUpdate != null)
            {
                recipeToUpdate.IsAvailable = false;
            }
        }
    }
    public async Task<PaginatedList<HerbalistAiRecipeResponse>> GetAllAiRecipeInventoryByAdminAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.HerbalistAiRecipeRepository.GetQueryable(tracked: false);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(h =>
                h.AiRecipe.RecommendedRecipeName.ToLower().Contains(search) || 
                h.Herbalist.User!.FullName.ToLower().Contains(search));
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "recipename" => isDesc ? query.OrderByDescending(h => h.AiRecipe.RecommendedRecipeName) : query.OrderBy(h => h.AiRecipe.RecommendedRecipeName),
                "price" => isDesc ? query.OrderByDescending(h => h.Price) : query.OrderBy(h => h.Price),
                "herbalistname" => isDesc ? query.OrderByDescending(h => h.Herbalist.User!.FullName) : query.OrderBy(h => h.Herbalist.User.FullName),
                _ => isDesc ? query.OrderByDescending(h => h.AiRecipe.RecommendedRecipeName) : query.OrderBy(h => h.AiRecipe.RecommendedRecipeName)
            };
        }
        else
        {
            query = isDesc ? query.OrderByDescending(h => h.AiRecipe.RecommendedRecipeName) : query.OrderBy(h => h.AiRecipe.RecommendedRecipeName);
        }

        var projectedQuery = query.ProjectTo<HerbalistAiRecipeResponse>(mapper.ConfigurationProvider);

        return await PaginatedList<HerbalistAiRecipeResponse>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);
    }

    public async Task<bool> RemoveAiRecipeByAdminAsync(int herbalistId, int aiRecipeId, CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.HerbalistAiRecipeRepository.GetAsync(
            filter: h => h.AiRecipeId == aiRecipeId && h.HerbalistId == herbalistId,
            tracked: true,
            cancellationToken: cancellationToken);
         
        if (item == null) return false;

        unitOfWork.HerbalistAiRecipeRepository.Remove(item);

        await CheckAndUpdateAvailability(aiRecipeId, herbalistId, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}