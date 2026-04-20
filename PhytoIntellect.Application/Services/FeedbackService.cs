using AutoMapper;
using PhytoIntellect.Application.Contracts.Feedbacks;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Application.Services;

public class FeedbackService(IUnitOfWork unitOfWork, IMapper mapper) : IFeedbackService
{
    public async Task<FeedbackResponse> SubmitRecipeFeedbackAsync(int userId, int recipeId, SubmitFeedbackRequest request, CancellationToken cancellationToken = default)
    {
        int patientId = await unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        if (patientId == 0) throw new Exception("Patient not found.");

        var recipe = await unitOfWork.RecipeRepository.GetAsync(r => r.RecipeId == recipeId && r.IsActive, tracked: true, cancellationToken: cancellationToken);
        if (recipe == null) throw new Exception("Recipe not found.");

        var existingFeedback = await unitOfWork.FeedbackRepository.GetAsync(f => f.RecipeId == recipeId && f.PatientId == patientId, tracked: true, cancellationToken: cancellationToken);

        Feedback feedbackEntity;
        float cleanRating = (float)Math.Round(request.RatingValue, 1);

        if (existingFeedback != null)
        {
            float oldRating = existingFeedback.RatingValue;
            existingFeedback.RatingValue = cleanRating;
            existingFeedback.Comment = request.Comment;
            existingFeedback.RatingDate = DateTime.UtcNow;

            var calc = ((recipe.AverageRating * recipe.TotalRatings) - oldRating + cleanRating) / recipe.TotalRatings;
            recipe.AverageRating = (float)Math.Round(calc, 1);
            feedbackEntity = existingFeedback;
        }
        else
        {
            feedbackEntity = new Feedback
            {
                RecipeId = recipeId,
                AiRecipeId = null, 
                PatientId = patientId,
                RatingValue = cleanRating,
                Comment = request.Comment,
                RatingDate = DateTime.UtcNow
            };
            await unitOfWork.FeedbackRepository.CreateAsync(feedbackEntity, cancellationToken);

            var calc = ((recipe.AverageRating * recipe.TotalRatings) + cleanRating) / (recipe.TotalRatings + 1);
            recipe.AverageRating = (float)Math.Round(calc, 1);
            recipe.TotalRatings += 1;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return mapper.Map<FeedbackResponse>(feedbackEntity);
    }

    public async Task<IEnumerable<FeedbackResponse>> GetRecipeFeedbacksAsync(int recipeId, CancellationToken cancellationToken = default)
    {
        var feedbacks = await unitOfWork.FeedbackRepository.GetAllAsync(filter: f => f.RecipeId == recipeId, tracked: false, includeProperties: "Patient.User", cancellationToken: cancellationToken);
        return mapper.Map<IEnumerable<FeedbackResponse>>(feedbacks).OrderByDescending(f => f.RatingDate);
    }

    public async Task<FeedbackResponse?> GetMyRecipeFeedbackAsync(int userId, int recipeId, CancellationToken cancellationToken = default)
    {
        int patientId = await unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        var feedback = await unitOfWork.FeedbackRepository.GetAsync(f => f.RecipeId == recipeId && f.PatientId == patientId, tracked: false, includeProperties: "Patient.User", cancellationToken: cancellationToken);
        return feedback == null ? null : mapper.Map<FeedbackResponse>(feedback);
    }

    public async Task<bool> DeleteMyRecipeFeedbackAsync(int userId, int recipeId, CancellationToken cancellationToken = default)
    {
        int patientId = await unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        var feedback = await unitOfWork.FeedbackRepository.GetAsync(f => f.RecipeId == recipeId && f.PatientId == patientId, tracked: true, cancellationToken: cancellationToken);
        if (feedback == null) return false;

        var recipe = await unitOfWork.RecipeRepository.GetAsync(r => r.RecipeId == recipeId, tracked: true, cancellationToken: cancellationToken);
        if (recipe != null)
        {
            if (recipe.TotalRatings == 1) { recipe.AverageRating = 0; recipe.TotalRatings = 0; }
            else
            {
                var calc = ((recipe.AverageRating * recipe.TotalRatings) - feedback.RatingValue) / (recipe.TotalRatings - 1);
                recipe.AverageRating = (float)Math.Round(calc, 1);
                recipe.TotalRatings -= 1;
            }
        }
        unitOfWork.FeedbackRepository.Remove(feedback);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    // (AiRecipeId)
    public async Task<FeedbackResponse> SubmitAiRecipeFeedbackAsync(int userId, int aiRecipeId, SubmitFeedbackRequest request, CancellationToken cancellationToken = default)
    {
        int patientId = await unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        if (patientId == 0) throw new UnauthorizedAccessException("Patient not found.");

        var aiRecipe = await unitOfWork.AiRecipeRepository.GetAsync(r => r.Id == aiRecipeId, tracked: true, cancellationToken: cancellationToken);
        if (aiRecipe == null) throw new KeyNotFoundException("AI Recipe not found.");

        if (aiRecipe.PatientId != patientId)
            throw new UnauthorizedAccessException("You are not authorized to evaluate an AI prescription for another patient.");

        var existingFeedback = await unitOfWork.FeedbackRepository.GetAsync(f => f.AiRecipeId == aiRecipeId && f.PatientId == patientId, tracked: true, cancellationToken: cancellationToken);

        Feedback feedbackEntity;
        float cleanRating = (float)Math.Round(request.RatingValue, 1);

        if (existingFeedback != null)
        {
            existingFeedback.RatingValue = cleanRating;
            existingFeedback.Comment = request.Comment;
            existingFeedback.RatingDate = DateTime.UtcNow;

            aiRecipe.Rating = cleanRating;
            feedbackEntity = existingFeedback;
        }
        else
        {
            feedbackEntity = new Feedback
            {
                RecipeId = null,
                AiRecipeId = aiRecipeId,
                PatientId = patientId,
                RatingValue = cleanRating,
                Comment = request.Comment,
                RatingDate = DateTime.UtcNow
            };
            await unitOfWork.FeedbackRepository.CreateAsync(feedbackEntity, cancellationToken);

            aiRecipe.Rating = cleanRating;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return mapper.Map<FeedbackResponse>(feedbackEntity);
    }

    public async Task<IEnumerable<FeedbackResponse>> GetAiRecipeFeedbacksAsync(int aiRecipeId, CancellationToken cancellationToken = default)
    {
        var feedbacks = await unitOfWork.FeedbackRepository.GetAllAsync(filter: f => f.AiRecipeId == aiRecipeId, tracked: false, includeProperties: "Patient.User", cancellationToken: cancellationToken);
        return mapper.Map<IEnumerable<FeedbackResponse>>(feedbacks).OrderByDescending(f => f.RatingDate);
    }

    public async Task<FeedbackResponse?> GetMyAiRecipeFeedbackAsync(int userId, int aiRecipeId, CancellationToken cancellationToken = default)
    {
        int patientId = await unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        var feedback = await unitOfWork.FeedbackRepository.GetAsync(f => f.AiRecipeId == aiRecipeId && f.PatientId == patientId, tracked: false, includeProperties: "Patient.User", cancellationToken: cancellationToken);
        return feedback == null ? null : mapper.Map<FeedbackResponse>(feedback);
    }

    public async Task<bool> DeleteMyAiRecipeFeedbackAsync(int userId, int aiRecipeId, CancellationToken cancellationToken = default)
    {
        int patientId = await unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        if (patientId == 0) throw new UnauthorizedAccessException("Patient not found.");

        var feedback = await unitOfWork.FeedbackRepository.GetAsync(
            f => f.AiRecipeId == aiRecipeId && f.PatientId == patientId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (feedback == null) return false;

        var aiRecipe = await unitOfWork.AiRecipeRepository.GetAsync(
            r => r.Id == aiRecipeId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (aiRecipe != null)
        {
            aiRecipe.Rating = null;
        }

        unitOfWork.FeedbackRepository.Remove(feedback);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<FeedbackResponse>> GetMyFeedbacksAsync(int userId, CancellationToken cancellationToken = default)
    {
        // 1. نجيب رقم المريض
        int patientId = await unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        if (patientId == 0) return new List<FeedbackResponse>();

        // 2. نجيب كل تقييماته سواء كانت لوصفات AI أو وصفات عطارين
        var feedbacks = await unitOfWork.FeedbackRepository.GetAllAsync(
            filter: f => f.PatientId == patientId,
            tracked: false,
            includeProperties: "Patient.User",
            cancellationToken: cancellationToken);

        // 3. نحولها لـ DTO ونرتبها من الأحدث للأقدم
        return mapper.Map<IEnumerable<FeedbackResponse>>(feedbacks).OrderByDescending(f => f.RatingDate).ToList();
    }
}