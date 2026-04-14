using PhytoIntellect.Application.Contracts.AiRecipes;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IAiPredictionService
{
    Task<AiPredictionResult> GetPredictionAsync(CreateAiRecipeRequest request);
}