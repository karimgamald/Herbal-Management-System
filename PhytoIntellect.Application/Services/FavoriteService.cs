using AutoMapper;
using AutoMapper.QueryableExtensions;
using PhytoIntellect.Application.Contracts.AiRecipes;
using PhytoIntellect.Application.Contracts.Herbalists;
using PhytoIntellect.Application.Contracts.Herbs;
using PhytoIntellect.Application.Contracts.Recipes;
using PhytoIntellect.Application.Contracts.UserFavorites;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
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

    public async Task<PaginatedList<FavoriteResponse>> GetMyFavoriteHerbsAsync(
    int userId,
    RequestFilters filters,
    CancellationToken cancellationToken = default)
    {
        var ids = await GetFavoriteIds(userId, FavoriteType.Herb);

        if (!ids.Any())
            return new PaginatedList<FavoriteResponse>(new List<FavoriteResponse>(), filters.PageNumber, 0, filters.PageSize);

        var query = unitOfWork.HerbRepository.GetQueryable(tracked: false)
            .Where(h => ids.Contains(h.HerbId));

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

        var projectedQuery = query.ProjectTo<FavoriteResponse>(mapper.ConfigurationProvider);
        return await PaginatedList<FavoriteResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }

    public async Task<PaginatedList<FavoriteResponse>> GetMyFavoriteRecipesAsync(
     int userId,
     RequestFilters filters,
     CancellationToken cancellationToken = default)
    {
        var ids = await GetFavoriteIds(userId, FavoriteType.Recipe);
        if (!ids.Any())
            return new PaginatedList<FavoriteResponse>(new List<FavoriteResponse>(), filters.PageNumber, 0, filters.PageSize);

        var query = unitOfWork.RecipeRepository.GetQueryable(tracked: false)
            .Where(r => ids.Contains(r.RecipeId));

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(r => r.RecipeDiseases.Any(rd => rd.Disease.DiseaseName.ToLower().Contains(search)));
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";

        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "averagerating" => isDesc ? query.OrderByDescending(r => r.AverageRating) : query.OrderBy(r => r.AverageRating),
                "createddate" => isDesc ? query.OrderByDescending(r => r.CreatedDate) : query.OrderBy(r => r.CreatedDate),
                "price" => isDesc ? query.OrderByDescending(r => r.Price) : query.OrderBy(r => r.Price),
                _ => isDesc ? query.OrderByDescending(r => r.AverageRating) : query.OrderBy(r => r.AverageRating)
            };
        }
        else
        {
            query = isDesc ? query.OrderByDescending(r => r.AverageRating) : query.OrderBy(r => r.AverageRating);
        }

        var projectedQuery = query.ProjectTo<FavoriteResponse>(mapper.ConfigurationProvider);
        return await PaginatedList<FavoriteResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }

    public async Task<PaginatedList<FavoriteResponse>> GetMyFavoriteAiRecipesAsync(
     int userId,
     RequestFilters filters,
     CancellationToken cancellationToken = default)
    {
        var ids = await GetFavoriteIds(userId, FavoriteType.AiRecipe);
        if (!ids.Any())
            return new PaginatedList<FavoriteResponse>(new List<FavoriteResponse>(), filters.PageNumber, 0, filters.PageSize);

        var query = unitOfWork.AiRecipeRepository.GetQueryable(tracked: false)
            .Where(r => ids.Contains(r.Id));

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(r => r.RecommendedRecipeName.ToLower().Contains(search));
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "recommendedrecipename" => isDesc ? query.OrderByDescending(r => r.RecommendedRecipeName) : query.OrderBy(r => r.RecommendedRecipeName),
                "confidencescore" => isDesc ? query.OrderByDescending(r => r.ConfidenceScore) : query.OrderBy(r => r.ConfidenceScore),
                _ => isDesc ? query.OrderByDescending(r => r.RecommendedRecipeName) : query.OrderBy(r => r.RecommendedRecipeName)
            };
        }
        else
        {
            query = isDesc ? query.OrderByDescending(r => r.RecommendedRecipeName) : query.OrderBy(r => r.RecommendedRecipeName);
        }

        var projectedQuery = query.ProjectTo<FavoriteResponse>(mapper.ConfigurationProvider);
        return await PaginatedList<FavoriteResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }

    public async Task<PaginatedList<FavoriteResponse>> GetMyFavoriteHerbalistsAsync(
    int userId,
    RequestFilters filters,
    CancellationToken cancellationToken = default)
    {
        var ids = await GetFavoriteIds(userId, FavoriteType.Herbalist);
        if (!ids.Any())
            return new PaginatedList<FavoriteResponse>(new List<FavoriteResponse>(), filters.PageNumber, 0, filters.PageSize);

        var query = unitOfWork.HerbalistRepository.GetQueryable(tracked: false)
            .Where(h => ids.Contains(h.HerbalistId));

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(h => h.User!.FullName.ToLower().Contains(search));
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "fullname" => isDesc ? query.OrderByDescending(h => h.User!.FullName) : query.OrderBy(h => h.User!.FullName),
                "licensenumber" => isDesc ? query.OrderByDescending(h => h.LicenseNumber) : query.OrderBy(h => h.LicenseNumber),
                "averagerating" => isDesc ? query.OrderByDescending(h => h.AverageRating) : query.OrderBy(h => h.AverageRating),
                _ => isDesc ? query.OrderByDescending(h => h.User!.FullName) : query.OrderBy(h => h.User!.FullName)
            };
        }
        else
        {
            query = isDesc ? query.OrderByDescending(h => h.User!.FullName) : query.OrderBy(h => h.User!.FullName);
        }

        var projectedQuery = query.ProjectTo<FavoriteResponse>(mapper.ConfigurationProvider);
        return await PaginatedList<FavoriteResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }

    public async Task<PaginatedList<FavoriteResponse>> GetMyFavoriteAiChatRecipesAsync(
     int userId,
     RequestFilters filters,
     CancellationToken cancellationToken = default)
    {
        var ids = await GetFavoriteIds(userId, FavoriteType.AiChatRecipe);
        if (!ids.Any())
            return new PaginatedList<FavoriteResponse>(new List<FavoriteResponse>(), filters.PageNumber, 0, filters.PageSize);

        var query = unitOfWork.AiChatRecipeRepository.GetQueryable(tracked: false)
            .Where(r => ids.Contains(r.Id));

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(r => r.RecommendedRecipeName.ToLower().Contains(search));
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "recommendedrecipename" => isDesc ? query.OrderByDescending(r => r.RecommendedRecipeName) : query.OrderBy(r => r.RecommendedRecipeName),
                "matchpercentage" => isDesc ? query.OrderByDescending(r => r.MatchPercentage) : query.OrderBy(r => r.MatchPercentage),
                "date" => isDesc ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt),
                _ => isDesc ? query.OrderByDescending(r => r.RecommendedRecipeName) : query.OrderBy(r => r.RecommendedRecipeName)
            };
        }
        else
        {
            query = isDesc ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt);
        }

        var projectedQuery = query.ProjectTo<FavoriteResponse>(mapper.ConfigurationProvider);
        return await PaginatedList<FavoriteResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
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
            FavoriteType.AiChatRecipe => await unitOfWork.AiChatRecipeRepository.GetAsync(r => r.Id == targetId, tracked: false) != null,
            FavoriteType.Herbalist => await unitOfWork.HerbalistRepository.GetAsync(h => h.HerbalistId == targetId, tracked: false) != null,
            _ => false
        };
    }
}