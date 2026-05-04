using PhytoIntellect.Application.Contracts.HerbalistAiRecipes;
using PhytoIntellect.Application.Paginations;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IHerbalistAiRecipeService
{
    Task<PaginatedList<HerbalistAiRecipeResponse>> GetMyInventoryAsync(int userId, RequestFilters filters, CancellationToken cancellationToken);
    Task<HerbalistAiRecipeResponse?> AddAiRecipeAsync(int userId, AddAiRecipeToInventoryRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdatePriceAsync(int userId, int aiRecipeId, decimal newPrice, CancellationToken cancellationToken = default);
    Task<bool> ToggleStatusAsync(int userId, int aiRecipeId, CancellationToken cancellationToken = default);
    Task<bool> RemoveAiRecipeAsync(int userId, int aiRecipeId, CancellationToken cancellationToken = default);

    Task<IEnumerable<HerbalistWithAiRecipeResponse>> GetHerbalistsByAiRecipeAsync(int aiRecipeId, bool isActive = true, CancellationToken cancellationToken = default);
}
