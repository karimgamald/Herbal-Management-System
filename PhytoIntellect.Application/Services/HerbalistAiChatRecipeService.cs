using AutoMapper;
using AutoMapper.QueryableExtensions;
using PhytoIntellect.Application.Contracts.HerbalistAiChatRecipes;
using PhytoIntellect.Application.Contracts.HerbalistAiRecipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Services;

public class HerbalistAiChatRecipeService(IUnitOfWork unitOfWork, IMapper mapper) : IHerbalistAiChatRecipeService
{
    public async Task<PaginatedList<HerbalistAiChatRecipeResponse>> GetMyInventoryAsync(
     int userId,
     RequestFilters filters,
     CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null) throw new KeyNotFoundException("Herbalist not found.");

        var query = unitOfWork.HerbalistAiChatRecipeRepository.GetQueryable(tracked: false);

        query = query.Where(h => h.HerbalistId == herbalist.HerbalistId);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(h => h.AiChatRecipe.RecommendedRecipeName.ToLower().Contains(search));
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "recommendedrecipename" => isDesc ? query.OrderByDescending(h => h.AiChatRecipe.RecommendedRecipeName) : query.OrderBy(h => h.AiChatRecipe.RecommendedRecipeName),
                "price" => isDesc ? query.OrderByDescending(h => h.Price) : query.OrderBy(h => h.Price),
                _ => isDesc ? query.OrderByDescending(h => h.AiChatRecipe.RecommendedRecipeName) : query.OrderBy(h => h.AiChatRecipe.RecommendedRecipeName)
            };
        }
        else
        {
            query = isDesc ? query.OrderByDescending(h => h.AiChatRecipe.RecommendedRecipeName) : query.OrderBy(h => h.AiChatRecipe.RecommendedRecipeName);
        }

        var projectedQuery = query.ProjectTo<HerbalistAiChatRecipeResponse>(mapper.ConfigurationProvider);

        return await PaginatedList<HerbalistAiChatRecipeResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }

    public async Task<HerbalistAiChatRecipeResponse?> AddAiChatRecipeAsync(int userId, AddAiChatRecipeToInventoryRequest request, CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null) throw new KeyNotFoundException("Herbalist not found.");

        var aiChatRecipe = await unitOfWork.AiChatRecipeRepository.GetAsync(
            filter: h => h.Id == request.AiChatRecipeId, 
            tracked: true,
            cancellationToken: cancellationToken);

        if (aiChatRecipe == null) throw new KeyNotFoundException("This AI Chat recipe does not exist in the system.");

        var existingItem = await unitOfWork.HerbalistAiChatRecipeRepository.GetAsync(
            filter: h => h.AiChatRecipeId == request.AiChatRecipeId && h.HerbalistId == herbalist.HerbalistId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (existingItem != null)
            throw new InvalidOperationException("This AI Chat recipe is already in your inventory. You can update its price instead.");

        var entity = new HerbalistAiChatRecipe
        {
            HerbalistId = herbalist.HerbalistId,
            AiChatRecipeId = request.AiChatRecipeId,
            Price = request.Price,
            IsActive = true
        };

        await unitOfWork.HerbalistAiChatRecipeRepository.CreateAsync(entity, cancellationToken);

        aiChatRecipe.IsAvailable = true;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var createdItem = await unitOfWork.HerbalistAiChatRecipeRepository.GetAsync(
            filter: h => h.AiChatRecipeId == request.AiChatRecipeId && h.HerbalistId == herbalist.HerbalistId,
            includeProperties: "AiChatRecipe",
            cancellationToken: cancellationToken);

        return mapper.Map<HerbalistAiChatRecipeResponse>(createdItem);
    }

    public async Task<bool> UpdatePriceAsync(int userId, int aiChatRecipeId, decimal newPrice, CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null) throw new KeyNotFoundException("Herbalist not found.");

        var item = await unitOfWork.HerbalistAiChatRecipeRepository.GetAsync(
            filter: h => h.AiChatRecipeId == aiChatRecipeId && h.HerbalistId == herbalist.HerbalistId, tracked: true, cancellationToken: cancellationToken);
        if (item == null) throw new KeyNotFoundException("AI Chat recipe not found in your inventory.");

        item.Price = newPrice;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ToggleStatusAsync(int userId, int aiChatRecipeId, CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(filter: h => h.UserId == userId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null) throw new KeyNotFoundException("Herbalist not found.");

        var item = await unitOfWork.HerbalistAiChatRecipeRepository.GetAsync(
            filter: h => h.AiChatRecipeId == aiChatRecipeId && h.HerbalistId == herbalist.HerbalistId, tracked: true, cancellationToken: cancellationToken);
        if (item == null) throw new KeyNotFoundException("AI Chat recipe not found in your inventory.");

        item.IsActive = !item.IsActive;

        if (!item.IsActive)
        {
            await CheckAndUpdateAvailability(aiChatRecipeId, herbalist.HerbalistId, cancellationToken);
        } 
        else
        {
            var recipe = await unitOfWork.AiChatRecipeRepository.GetAsync(r => r.Id == aiChatRecipeId, tracked: true);
            if (recipe != null) recipe.IsAvailable = true;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return item.IsActive;
    }

    public async Task<bool> RemoveAiChatRecipeAsync(int userId, int aiChatRecipeId, CancellationToken cancellationToken)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(filter: h => h.UserId == userId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null) throw new KeyNotFoundException("Herbalist not found.");

        var item = await unitOfWork.HerbalistAiChatRecipeRepository.GetAsync(
            filter: h => h.AiChatRecipeId == aiChatRecipeId && h.HerbalistId == herbalist.HerbalistId, tracked: true, cancellationToken: cancellationToken);
        if (item == null) throw new KeyNotFoundException("AI Chat recipe not found in your inventory.");

        unitOfWork.HerbalistAiChatRecipeRepository.Remove(item);

        await CheckAndUpdateAvailability(aiChatRecipeId, herbalist.HerbalistId, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<HerbalistWithAiChatRecipeResponse>> GetHerbalistsByAiChatRecipeAsync(int aiChatRecipeId, bool isActive = true, CancellationToken cancellationToken = default)
    {
        var herbalistRecipes = await unitOfWork.HerbalistAiChatRecipeRepository.GetAllAsync(
            filter: h => h.AiChatRecipeId == aiChatRecipeId && h.IsActive == isActive,
            tracked: false,
            includeProperties: "Herbalist.User",
            cancellationToken: cancellationToken);

        return mapper.Map<IEnumerable<HerbalistWithAiChatRecipeResponse>>(herbalistRecipes);
    }

    private async Task CheckAndUpdateAvailability(int aiChatRecipeId, int excludedHerbalistId, CancellationToken cancellationToken)
    {
        var anyOtherActive = await unitOfWork.HerbalistAiChatRecipeRepository.GetAsync(
            filter: h => h.AiChatRecipeId == aiChatRecipeId && h.IsActive && h.HerbalistId != excludedHerbalistId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (anyOtherActive == null)
        {
            var recipeToUpdate = await unitOfWork.AiChatRecipeRepository.GetAsync(
                filter: r => r.Id == aiChatRecipeId,
                tracked: true,
                cancellationToken: cancellationToken);

            if (recipeToUpdate != null)
            {
                recipeToUpdate.IsAvailable = false;
            }
        }
    }

    public async Task<PaginatedList<HerbalistAiChatRecipeResponse>> AdminGetAllInventoryAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.HerbalistAiChatRecipeRepository.GetQueryable(tracked: false);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(h =>
                h.AiChatRecipe.RecommendedRecipeName.ToLower().Contains(search) ||
                h.Herbalist.User!.FullName.ToLower().Contains(search));
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "recommendedrecipename" => isDesc ? query.OrderByDescending(h => h.AiChatRecipe.RecommendedRecipeName) : query.OrderBy(h => h.AiChatRecipe.RecommendedRecipeName),
                "price" => isDesc ? query.OrderByDescending(h => h.Price) : query.OrderBy(h => h.Price),
                "herbalistname" => isDesc ? query.OrderByDescending(h => h.Herbalist.User!.FullName) : query.OrderBy(h => h.Herbalist.User.FullName),
                _ => isDesc ? query.OrderByDescending(h => h.AiChatRecipe.RecommendedRecipeName) : query.OrderBy(h => h.AiChatRecipe.RecommendedRecipeName)
            };
        }
        else
        {
            query = isDesc ? query.OrderByDescending(h => h.AiChatRecipe.RecommendedRecipeName) : query.OrderBy(h => h.AiChatRecipe.RecommendedRecipeName);
        }

        var projectedQuery = query.ProjectTo<HerbalistAiChatRecipeResponse>(mapper.ConfigurationProvider);
        return await PaginatedList<HerbalistAiChatRecipeResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }

    public async Task<bool> AdminRemoveAiChatRecipeAsync(int herbalistId, int aiChatRecipeId, CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.HerbalistAiChatRecipeRepository.GetAsync(
            filter: h => h.AiChatRecipeId == aiChatRecipeId && h.HerbalistId == herbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (item == null) return false;

        unitOfWork.HerbalistAiChatRecipeRepository.Remove(item);

        await CheckAndUpdateAvailability(aiChatRecipeId, herbalistId, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

}