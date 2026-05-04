using PhytoIntellect.Application.Contracts.Feedbacks;
using PhytoIntellect.Application.Paginations;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IFeedbackService
{
    // 🌿 1. (RecipeId)
    Task<FeedbackResponse> SubmitRecipeFeedbackAsync(int userId, int recipeId, SubmitFeedbackRequest request, CancellationToken cancellationToken = default);
    Task<PaginatedList<FeedbackResponse>> GetRecipeFeedbacksAsync(int recipeId, RequestFilters filters, CancellationToken cancellationToken = default);
    Task<FeedbackResponse?> GetMyRecipeFeedbackAsync(int userId, int recipeId, CancellationToken cancellationToken = default);
    Task<bool> DeleteMyRecipeFeedbackAsync(int userId, int recipeId, CancellationToken cancellationToken = default);

    // 🤖 2. (AiRecipeId)
    Task<FeedbackResponse> SubmitAiRecipeFeedbackAsync(int userId, int aiRecipeId, SubmitFeedbackRequest request, CancellationToken cancellationToken = default);
    Task<PaginatedList<FeedbackResponse>> GetAiRecipeFeedbacksAsync(int aiRecipeId, RequestFilters filters, CancellationToken cancellationToken = default);
    Task<FeedbackResponse?> GetMyAiRecipeFeedbackAsync(int userId, int aiRecipeId, CancellationToken cancellationToken = default);
    Task<bool> DeleteMyAiRecipeFeedbackAsync(int userId, int aiRecipeId, CancellationToken cancellationToken = default);

    // 👤 دالة سجل تقييمات المريض (شاشة البروفايل)
    Task<PaginatedList<FeedbackResponse>> GetMyFeedbacksAsync(int userId, RequestFilters filters, CancellationToken cancellationToken = default);

}
