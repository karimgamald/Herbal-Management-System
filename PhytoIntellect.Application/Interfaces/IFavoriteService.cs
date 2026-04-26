using PhytoIntellect.Application.Contracts.AiRecipes;
using PhytoIntellect.Application.Contracts.Herbalists;
using PhytoIntellect.Application.Contracts.Herbs;
using PhytoIntellect.Application.Contracts.Recipes;
using PhytoIntellect.Application.Contracts.UserFavorites;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IFavoriteService
{
    Task<string> ToggleFavoriteAsync(int userId, ToggleFavoriteRequest request);
    Task<IEnumerable<FavoriteResponse>> GetMyFavoriteHerbsAsync(int userId);
    Task<IEnumerable<FavoriteResponse>> GetMyFavoriteRecipesAsync(int userId);
    Task<IEnumerable<FavoriteResponse>> GetMyFavoriteAiRecipesAsync(int userId); 
    Task<IEnumerable<FavoriteResponse>> GetMyFavoriteHerbalistsAsync(int userId);
}