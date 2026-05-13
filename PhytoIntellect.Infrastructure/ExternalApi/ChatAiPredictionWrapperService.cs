using PhytoIntellect.Application.Contracts.ChatAiRecipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Infrastructure.ExternalApi.ChatContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.ExternalApi;

public class ChatAiPredictionWrapperService(IChatAiClient chatClient) : IChatAiPredictionService
{
    private readonly IChatAiClient _chatClient = chatClient;

    public async Task<AiChatPredictionResult> GetChatPredictionAsync(string userPrompt, CancellationToken cancellationToken = default)
    {
        var flaskRequest = new FlaskChatAiRequest
        {
            Prompt = userPrompt
        };

        var flaskResponse = await _chatClient.GetChatPredictionAsync(flaskRequest, cancellationToken);

        var topPrediction = flaskResponse?.Predictions?.FirstOrDefault();

        if (topPrediction == null)
        {
            throw new Exception("The AI model did not return any predictions.");
        }

        var otherOptions = flaskResponse!.Predictions
            .Skip(1)
            .Select(p => $"{p.RecipeName} - {Math.Round(p.Confidence * 100)}%")
            .ToList();

        return new AiChatPredictionResult
        {
            RecommendedRecipeName = topPrediction.RecipeName,
            MainHerb = topPrediction.MainHerb,
            ScientificName = topPrediction.ScientificName,
            Category = topPrediction.Category,
            Preparation = topPrediction.Preparation,
            Dosage = topPrediction.Dosage,
            Contraindications = topPrediction.Contraindications,
            MatchPercentage = Math.Round(topPrediction.Confidence * 100),

            OtherPossibilities = otherOptions
        };
    }
}