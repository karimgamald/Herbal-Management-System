using PhytoIntellect.Application.Contracts.AiRecipes;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IAiRecipeService
{
    Task<AiRecipeResponse> GenerateRecipeAsync(int userId, CreateAiRecipeRequest request);
} 