using PhytoIntellect.Application.Contracts.ChatAiRecipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Services;

public class ChatAiRecipeService(
    IChatAiPredictionService chatAiPredictionService,
    IUnitOfWork unitOfWork
    ): IChatAiRecipeService
{
    private readonly IChatAiPredictionService _chatAiPredictionService = chatAiPredictionService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<AiChatPredictionResult> GenerateChatRecipeAsync(int userId, CreateChatRecipeRequest request, CancellationToken cancellationToken)
    {
        int patientId = await _unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        if (patientId == 0) throw new UnauthorizedAccessException("Patient profile not found.");

        var patient = await _unitOfWork.PatientRepository.GetPatientWithHistoryAsync(patientId);

        if (patient == null)
            throw new Exception("Patient not found.");


        var predictionResult = await _chatAiPredictionService.GetChatPredictionAsync(request.UserPrompt, cancellationToken);

        var recipeRecord = new AiChatRecipe
        {
            PatientId = patientId,
            UserPrompt = request.UserPrompt,
            RecommendedRecipeName = predictionResult.RecommendedRecipeName,
            MainHerb = predictionResult.MainHerb,  
            Dosage = predictionResult.Dosage,
            MatchPercentage = predictionResult.MatchPercentage,
            Preparation = predictionResult.Preparation,
            ScientificName = predictionResult.ScientificName,
            Contraindications = predictionResult.Contraindications,
            OtherPossibilities = predictionResult.OtherPossibilities ?? [],
            Category = predictionResult.Category,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            IsAvailable = false
        };
        await _unitOfWork.AiChatRecipeRepository.CreateAsync(recipeRecord, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return predictionResult;
    }
}