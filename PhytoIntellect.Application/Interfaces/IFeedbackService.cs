using PhytoIntellect.Application.Contracts.Feedbacks;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IFeedbackService
{
    Task<FeedbackResponse> SubmitFeedbackAsync(int userId, int recipeId, SubmitFeedbackRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<FeedbackResponse>> GetRecipeFeedbacksAsync(int recipeId, CancellationToken cancellationToken = default);
    Task<FeedbackResponse?> GetMyFeedbackAsync(int userId, int recipeId, CancellationToken cancellationToken = default);
    Task<bool> DeleteMyFeedbackAsync(int userId, int recipeId, CancellationToken cancellationToken = default);
}