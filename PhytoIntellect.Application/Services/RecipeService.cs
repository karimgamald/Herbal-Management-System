using AutoMapper;
using PhytoIntellect.Application.Contracts.Recipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Application.Services;

public class RecipeService(IUnitOfWork unitOfWork, IMapper mapper) : IRecipeService
{
    // 1. إنشاء وصفة جديدة
    public async Task<RecipeResponse?> AddRecipeAsync(int userId, CreateRecipeRequest request, 
        CancellationToken cancellationToken = default)
    {
        // استخدام Named Arguments عشان نتفادى أي لغبطة في ترتيب الباراميترز
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            throw new Exception("Herbalist profile not found.");

        var recipe = mapper.Map<Recipe>(request);
        recipe.HerbalistId = herbalist.HerbalistId;
        recipe.CreatedDate = DateTime.UtcNow;

        await unitOfWork.RecipeRepository.CreateAsync(recipe, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var createdRecipe = await unitOfWork.RecipeRepository.GetAsync(
            filter: r => r.RecipeId == recipe.RecipeId,
            tracked: false,
            includeProperties: "RecipeHerbs.Herb",
            cancellationToken: cancellationToken);

        return mapper.Map<RecipeResponse>(createdRecipe);
    }

    // 2. تصفح كل الوصفات المتاحة
    public async Task<IEnumerable<RecipeResponse>> GetAllActiveRecipesAsync(CancellationToken cancellationToken = default)
    {
        var recipes = await unitOfWork.RecipeRepository.GetAllAsync(
            filter: r => r.IsActive,
            tracked: false,
            includeProperties: "RecipeHerbs.Herb",
            cancellationToken: cancellationToken);

        return mapper.Map<IEnumerable<RecipeResponse>>(recipes);
    }

    // 3. جلب تفاصيل وصفة واحدة
    public async Task<RecipeResponse?> GetRecipeByIdAsync(int recipeId, CancellationToken cancellationToken = default)
    {
        var recipe = await unitOfWork.RecipeRepository.GetAsync(
            filter: r => r.RecipeId == recipeId && r.IsActive,
            tracked: false,
            includeProperties: "RecipeHerbs.Herb",
            cancellationToken: cancellationToken);

        return recipe == null ? null : mapper.Map<RecipeResponse>(recipe);
    }

    // 4. تعديل الوصفة
    public async Task<RecipeResponse?> UpdateRecipeAsync(int userId, int recipeId, UpdateRecipeRequest request, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null) throw new Exception("Herbalist not found.");

        var recipe = await unitOfWork.RecipeRepository.GetAsync(
            filter: r => r.RecipeId == recipeId,
            tracked: true, // لازم تبقى True عشان التعديل يسمع في الداتابيز
            includeProperties: "RecipeHerbs",
            cancellationToken: cancellationToken);

        if (recipe == null) throw new Exception("Recipe not found.");

        if (recipe.HerbalistId != herbalist.HerbalistId)
            throw new UnauthorizedAccessException("You can only update your own recipes.");

        recipe.Description = request.Description;
        recipe.Instructions = request.Instructions;

        recipe.RecipeHerbs.Clear();
        foreach (var herbReq in request.Herbs)
        {
            recipe.RecipeHerbs.Add(new RecipeHerb { HerbId = herbReq.HerbId, Quantity = herbReq.Quantity });
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetRecipeByIdAsync(recipe.RecipeId, cancellationToken);
    }

    // 5. مسح الوصفة (Soft Delete)
    public async Task<bool> DeleteRecipeAsync(int userId, int recipeId, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null) return false;

        var recipe = await unitOfWork.RecipeRepository.GetAsync(
            filter: r => r.RecipeId == recipeId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (recipe == null || recipe.HerbalistId != herbalist.HerbalistId) return false;

        recipe.IsActive = false;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}