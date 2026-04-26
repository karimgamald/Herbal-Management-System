using AutoMapper;
using PhytoIntellect.Application.Contracts.AiRecipes;
using PhytoIntellect.Application.Contracts.Herbalists;
using PhytoIntellect.Application.Contracts.Herbs;
using PhytoIntellect.Application.Contracts.Recipes;
using PhytoIntellect.Application.Contracts.UserFavorites;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhytoIntellect.Application.Services;

public class FavoriteService(IUnitOfWork unitOfWork, IMapper mapper) : IFavoriteService
{
    public async Task<string> ToggleFavoriteAsync(int userId, ToggleFavoriteRequest request)
    {
        var favoriteType = Enum.Parse<FavoriteType>(request.Type, ignoreCase: true);

        bool targetExists = await CheckTargetExistsAsync(request.TargetId, favoriteType);
        if (!targetExists)
        {
            return "Target not found.";
        }

        var existing = await unitOfWork.UserFavoriteRepository.GetAsync(
            f => f.UserId == userId && f.TargetId == request.TargetId && f.Type == favoriteType);

        if (existing != null)
        {
            unitOfWork.UserFavoriteRepository.Remove(existing);
            await unitOfWork.SaveChangesAsync();
            return "Removed from favorites successfully.";
        }

        var newFavorite = new UserFavorite
        {
            UserId = userId,
            TargetId = request.TargetId,
            Type = favoriteType
        };

        await unitOfWork.UserFavoriteRepository.CreateAsync(newFavorite);
        await unitOfWork.SaveChangesAsync();
        return "Added to favorites successfully.";
    }

    public async Task<IEnumerable<FavoriteResponse>> GetMyFavoriteHerbsAsync(int userId)
    {
        var ids = await GetFavoriteIds(userId, FavoriteType.Herb);
        if (!ids.Any()) return new List<FavoriteResponse>();

        var items = await unitOfWork.HerbRepository.GetAllAsync(h => ids.Contains(h.HerbId), tracked: false);
        return mapper.Map<IEnumerable<FavoriteResponse>>(items);
    }

    public async Task<IEnumerable<FavoriteResponse>> GetMyFavoriteRecipesAsync(int userId)
    {
        var ids = await GetFavoriteIds(userId, FavoriteType.Recipe);
        if (!ids.Any()) return new List<FavoriteResponse>();

        var items = await unitOfWork.RecipeRepository.GetAllAsync(
            filter: r => ids.Contains(r.RecipeId),
            includeProperties: "RecipeHerbs.Herb,RecipeDiseases.Disease",
            tracked: false);

        return mapper.Map<IEnumerable<FavoriteResponse>>(items);
    }

    public async Task<IEnumerable<FavoriteResponse>> GetMyFavoriteAiRecipesAsync(int userId)
    {
        var ids = await GetFavoriteIds(userId, FavoriteType.AiRecipe);
        if (!ids.Any()) return new List<FavoriteResponse>();

        var items = await unitOfWork.AiRecipeRepository.GetAllAsync(r => ids.Contains(r.Id), tracked: false);
        return mapper.Map<IEnumerable<FavoriteResponse>>(items);
    }

    public async Task<IEnumerable<FavoriteResponse>> GetMyFavoriteHerbalistsAsync(int userId)
    {
        var ids = await GetFavoriteIds(userId, FavoriteType.Herbalist);
        if (!ids.Any()) return new List<FavoriteResponse>();

        var items = await unitOfWork.HerbalistRepository.GetAllAsync(
            filter: h => ids.Contains(h.HerbalistId),
            includeProperties: "User",
            tracked: false);

        return mapper.Map<IEnumerable<FavoriteResponse>>(items);
    }

    // Helper Method 1: Get IDs
    private async Task<List<int>> GetFavoriteIds(int userId, FavoriteType type)
    {
        var favorites = await unitOfWork.UserFavoriteRepository.GetAllAsync(
            f => f.UserId == userId && f.Type == type,
            tracked: false);

        return favorites.Select(f => f.TargetId).ToList();
    }

    // 🛑 Helper Method 2 (NEW): التأكد من وجود العنصر
    private async Task<bool> CheckTargetExistsAsync(int targetId, FavoriteType type)
    {
        return type switch
        {
            FavoriteType.Herb => await unitOfWork.HerbRepository.GetAsync(h => h.HerbId == targetId, tracked: false) != null,
            FavoriteType.Recipe => await unitOfWork.RecipeRepository.GetAsync(r => r.RecipeId == targetId, tracked: false) != null,
            FavoriteType.AiRecipe => await unitOfWork.AiRecipeRepository.GetAsync(r => r.Id == targetId, tracked: false) != null,
            FavoriteType.Herbalist => await unitOfWork.HerbalistRepository.GetAsync(h => h.HerbalistId == targetId, tracked: false) != null,
            _ => false
        };
    }
}