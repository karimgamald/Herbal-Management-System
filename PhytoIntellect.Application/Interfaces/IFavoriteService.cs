using PhytoIntellect.Application.Contracts.AiRecipes;
using PhytoIntellect.Application.Contracts.Herbalists;
using PhytoIntellect.Application.Contracts.Herbs;
using PhytoIntellect.Application.Contracts.Recipes;
using PhytoIntellect.Application.Contracts.UserFavorites;
using PhytoIntellect.Application.Paginations;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IFavoriteService
{
    Task<string> ToggleFavoriteAsync(int userId, ToggleFavoriteRequest request);
    Task<PaginatedList<FavoriteResponse>> GetMyFavoriteHerbsAsync(int userId, RequestFilters filters, CancellationToken cancellationToken = default);
    Task<PaginatedList<FavoriteResponse>> GetMyFavoriteRecipesAsync(int userId, RequestFilters filters, CancellationToken cancellationToken = default);
    Task<PaginatedList<FavoriteResponse>> GetMyFavoriteAiRecipesAsync(int userId, RequestFilters filters, CancellationToken cancellationToken = default); 
    Task<PaginatedList<FavoriteResponse>> GetMyFavoriteHerbalistsAsync(int userId, RequestFilters filters, CancellationToken cancellationToken = default);
}