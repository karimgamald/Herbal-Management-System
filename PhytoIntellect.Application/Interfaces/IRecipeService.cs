using PhytoIntellect.Application.Contracts.Recipes;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IRecipeService
{
    Task<RecipeResponse?> AddRecipeAsync(int userId, CreateRecipeRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<RecipeResponse>> GetAllActiveRecipesAsync(CancellationToken cancellationToken = default);
    Task<RecipeResponse?> GetRecipeByIdAsync(int recipeId, CancellationToken cancellationToken = default);
    Task<RecipeResponse?> UpdateRecipeAsync(int userId, int recipeId, UpdateRecipeRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateRecipeAvailabilityAsync(int userId, int recipeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RecipeResponse>> GetRecipesByHerbalistIdAsync(int herbalistId, bool? isActive = null, CancellationToken cancellationToken = default);
}