using PhytoIntellect.Application.Contracts.Reviews;
using PhytoIntellect.Application.Paginations;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IReviewRecipeService
{
    Task<ReviewResponse> SubmitReviewAsync(int userId, int aiRecipeId, SubmitReviewRequest request, CancellationToken cancellationToken = default);
    Task<PaginatedList<ReviewResponse>> GetAllRecipeReviewsAsync(int aiRecipeId, RequestFilters filters, CancellationToken cancellationToken = default);
    Task<ReviewResponse?> GetMyReviewAsync(int userId, int aiRecipeId, CancellationToken cancellationToken = default);
    Task<bool> DeleteMyReviewAsync(int userId, int aiRecipeId, CancellationToken cancellationToken = default);
    Task<ReviewResponse> SubmitAiChatReviewAsync(int userId, int aiChatRecipeId, SubmitReviewRequest request, CancellationToken cancellationToken = default);
    Task<PaginatedList<ReviewResponse>> GetAllAiChatRecipeReviewsAsync(int aiChatRecipeId, RequestFilters filters, CancellationToken cancellationToken = default);
    Task<ReviewResponse?> GetMyAiChatReviewAsync(int userId, int aiChatRecipeId, CancellationToken cancellationToken = default);
    Task<bool> DeleteMyAiChatReviewAsync(int userId, int aiChatRecipeId, CancellationToken cancellationToken = default);

    // Admin Endpoints
    Task<PaginatedList<ReviewResponse>> GetAllSystemReviewsAsync(RequestFilters filters, CancellationToken cancellationToken = default);
    Task<bool> DeleteAnyReviewAsync(int reviewId, CancellationToken cancellationToken = default);

    Task<PaginatedList<ReviewResponse>> GetAllSystemAiChatReviewsAsync(RequestFilters filters, CancellationToken cancellationToken = default);
    Task<bool> DeleteAnyAiChatReviewAsync(int reviewId, CancellationToken cancellationToken = default);
}
