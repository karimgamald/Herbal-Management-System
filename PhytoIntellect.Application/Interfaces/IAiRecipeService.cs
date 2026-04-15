using PhytoIntellect.Application.Contracts.AiRecipes;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IAiRecipeService
{

    // 🔥 NEW: Public APIs
    Task<IEnumerable<AiRecipeResponse>> GetAllPublicAsync(CancellationToken cancellationToken = default);
    Task<AiRecipeResponse> GetPublicByIdAsync(int recipeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AiRecipeResponse>> GetAllAsync(int userId, CancellationToken cancellationToken = default);
    Task<AiRecipeResponse> GetByIdAsync(int userId, int recipeId, CancellationToken cancellationToken = default);
    Task<AiRecipeResponse> GenerateRecipeAsync(int userId, CreateAiRecipeRequest request);
    //Task DeleteAsync(int recipeId, CancellationToken cancellationToken = default);
}