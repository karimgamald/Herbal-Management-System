using PhytoIntellect.Application.Contracts.Feedbacks;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IFeedbackService
{
    // 🌿 1. (RecipeId)
    Task<FeedbackResponse> SubmitRecipeFeedbackAsync(int userId, int recipeId, SubmitFeedbackRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<FeedbackResponse>> GetRecipeFeedbacksAsync(int recipeId, CancellationToken cancellationToken = default);
    Task<FeedbackResponse?> GetMyRecipeFeedbackAsync(int userId, int recipeId, CancellationToken cancellationToken = default);
    Task<bool> DeleteMyRecipeFeedbackAsync(int userId, int recipeId, CancellationToken cancellationToken = default);

    // 🤖 2. (AiRecipeId)
    Task<FeedbackResponse> SubmitAiRecipeFeedbackAsync(int userId, int aiRecipeId, SubmitFeedbackRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<FeedbackResponse>> GetAiRecipeFeedbacksAsync(int aiRecipeId, CancellationToken cancellationToken = default);
    Task<FeedbackResponse?> GetMyAiRecipeFeedbackAsync(int userId, int aiRecipeId, CancellationToken cancellationToken = default);
    Task<bool> DeleteMyAiRecipeFeedbackAsync(int userId, int aiRecipeId, CancellationToken cancellationToken = default);

    // 👤 دالة سجل تقييمات المريض (شاشة البروفايل)
    Task<IEnumerable<FeedbackResponse>> GetMyFeedbacksAsync(int userId, CancellationToken cancellationToken = default);

}
