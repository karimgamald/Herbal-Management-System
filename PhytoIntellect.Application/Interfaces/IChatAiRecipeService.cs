using PhytoIntellect.Application.Contracts.ChatAiRecipes;

namespace PhytoIntellect.Application.Interfaces;

public interface IChatAiRecipeService
{
    Task<AiChatPredictionResult> GenerateChatRecipeAsync(int userId, CreateChatRecipeRequest request, CancellationToken cancellationToken = default);
}
