using PhytoIntellect.Application.Contracts.Recipes;
using PhytoIntellect.Application.Paginations;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IRecipeService
{
    Task<RecipeResponse?> AddRecipeAsync(int userId, CreateRecipeRequest request, CancellationToken cancellationToken = default);
    Task<PaginatedList<RecipeResponse>> GetAllActiveRecipesAsync(RequestFilters filters,CancellationToken cancellationToken = default);
    Task<RecipeResponse?> GetRecipeByIdAsync(int recipeId, CancellationToken cancellationToken = default);
    Task<RecipeResponse?> UpdateRecipeAsync(int userId, int recipeId, UpdateRecipeRequest request, CancellationToken cancellationToken = default);
    Task<PaginatedList<RecipeResponse>> GetRecipesByHerbalistIdAsync(int herbalistId, RequestFilters filters, bool? isActive = null, CancellationToken cancellationToken = default);
    Task<bool?> ToggleRecipeAvailabilityAsync(int userId, int recipeId, CancellationToken cancellationToken = default);
    Task<bool> DeactivateRecipeByAdminAsync(int recipeId, string reason, CancellationToken cancellationToken);
}