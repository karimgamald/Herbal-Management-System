using PhytoIntellect.Application.Contracts.Reviews;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IReviewRecipeService
{
    Task<ReviewResponse> SubmitReviewAsync(int userId, int aiRecipeId, SubmitReviewRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<ReviewResponse>> GetAllRecipeReviewsAsync(int aiRecipeId, CancellationToken cancellationToken = default);
    Task<ReviewResponse?> GetMyReviewAsync(int userId, int aiRecipeId, CancellationToken cancellationToken = default);
    Task<bool> DeleteMyReviewAsync(int userId, int aiRecipeId, CancellationToken cancellationToken = default);
}
