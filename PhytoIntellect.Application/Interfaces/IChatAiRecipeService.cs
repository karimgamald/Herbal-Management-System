using PhytoIntellect.Application.Contracts.ChatAiRecipes;
using PhytoIntellect.Application.Paginations;

namespace PhytoIntellect.Application.Interfaces;

public interface IChatAiRecipeService
{
    Task<AiChatPredictionResult> GenerateChatRecipeAsync(int userId, CreateChatRecipeRequest request, CancellationToken cancellationToken = default);
    Task<PaginatedList<AiChatPredictionResult>> GetAllPublicAsync(RequestFilters filters, CancellationToken cancellationToken = default);
    Task<AiChatPredictionResult> GetPublicByIdAsync(int recipeId, CancellationToken cancellationToken = default);
    Task<PaginatedList<AiChatPredictionResult>> GetPatientAllAsync(int userId, RequestFilters filters,
        CancellationToken cancellationToken = default);
    Task<AiChatPredictionResult> GetPatientRecipeByIdAsync(int userId, int recipeId, CancellationToken cancellationToken = default);
}
