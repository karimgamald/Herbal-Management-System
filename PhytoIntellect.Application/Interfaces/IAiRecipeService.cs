using PhytoIntellect.Application.Contracts.AiRecipes;
using PhytoIntellect.Application.Paginations;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IAiRecipeService
{

    // 🔥 NEW: Public APIs
    Task<PaginatedList<AiRecipeResponse>> GetAllPublicAsync(RequestFilters filters, CancellationToken cancellationToken = default);
    Task<AiRecipeResponse> GetPublicByIdAsync(int recipeId, CancellationToken cancellationToken = default);
    Task<PaginatedList<AiRecipeResponse>> GetAllAsync(int userId, RequestFilters filters, CancellationToken cancellationToken = default);
    Task<AiRecipeResponse> GetByIdAsync(int userId, int recipeId, CancellationToken cancellationToken = default);
    Task<AiRecipeResponse> GenerateRecipeAsync(int userId, CreateAiRecipeRequest request);
    //Task DeleteAsync(int recipeId, CancellationToken cancellationToken = default);
}