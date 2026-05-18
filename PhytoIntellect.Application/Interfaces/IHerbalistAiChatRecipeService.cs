using PhytoIntellect.Application.Contracts.HerbalistAiChatRecipes;
using PhytoIntellect.Application.Contracts.HerbalistAiRecipes;
using PhytoIntellect.Application.Paginations;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IHerbalistAiChatRecipeService
{
    Task<PaginatedList<HerbalistAiChatRecipeResponse>> GetMyInventoryAsync(int userId, RequestFilters filters, CancellationToken cancellationToken = default);
    Task<HerbalistAiChatRecipeResponse?> AddAiChatRecipeAsync(int userId, AddAiChatRecipeToInventoryRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdatePriceAsync(int userId, int aiChatRecipeId, decimal newPrice, CancellationToken cancellationToken);
    Task<bool> ToggleStatusAsync(int userId, int aiChatRecipeId, CancellationToken cancellationToken);
    Task<bool> RemoveAiChatRecipeAsync(int userId, int aiChatRecipeId, CancellationToken cancellationToken);
    Task<IEnumerable<HerbalistWithAiChatRecipeResponse>> GetHerbalistsByAiChatRecipeAsync(int aiChatRecipeId, bool isActive = true, CancellationToken cancellationToken = default);

    Task<PaginatedList<HerbalistAiChatRecipeResponse>> GetAllAiChatRecipeInventoryByAdminAsync(RequestFilters filters, CancellationToken cancellationToken = default);
    Task<bool> RemoveAiChatRecipeByAdminAsync(int herbalistId, int aiChatRecipeId, CancellationToken cancellationToken = default);
}