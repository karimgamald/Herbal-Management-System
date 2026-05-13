using PhytoIntellect.Application.Contracts.ChatAiRecipes;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IChatAiPredictionService
{
    Task<AiChatPredictionResult> GetChatPredictionAsync(string userPrompt, CancellationToken cancellationToken = default);
}