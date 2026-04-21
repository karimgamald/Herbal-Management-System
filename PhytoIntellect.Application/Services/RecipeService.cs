using AutoMapper;
using PhytoIntellect.Application.Contracts.Recipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;


namespace PhytoIntellect.Application.Services;

public class RecipeService(IUnitOfWork unitOfWork, IMapper mapper) : IRecipeService
{
    public async Task<RecipeResponse?> AddRecipeAsync(
    int userId,
    CreateRecipeRequest request,
    CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null)
            throw new Exception("Herbalist profile not found.");

        var normalizedInstructions = request.Instructions.Trim().ToLower();

        var existingRecipes = await unitOfWork.RecipeRepository.GetAllAsync(
            filter: r => r.HerbalistId == herbalist.HerbalistId,
            includeProperties: "RecipeHerbs,RecipeDiseases",
            tracked: false,
            cancellationToken: cancellationToken);

        // 🔥 التحقق من الهوية الأساسية فقط (بدون Price و Description)
        var isSameRecipe = existingRecipes.Any(r =>
            r.Instructions.Trim().ToLower() == normalizedInstructions &&

            r.RecipeHerbs.Count == request.Herbs.Count &&
            r.RecipeHerbs.All(h =>
                request.Herbs.Any(req =>
                    req.HerbId == h.HerbId &&
                    req.Quantity == h.Quantity)) &&

            (
                (r.RecipeDiseases == null && request.DiseaseIds == null) ||
                (r.RecipeDiseases != null && request.DiseaseIds != null &&
                 r.RecipeDiseases.Count == request.DiseaseIds.Count &&
                 r.RecipeDiseases.All(d =>
                     request.DiseaseIds.Contains(d.DiseaseId)))
            )
        );

        // ❌ لو نفس الريسيبي (الهوية الأساسية متطابقة)
        if (isSameRecipe)
        {
            throw new Exception("Recipe already exists. or you can make update for price or description.");
        }

        // ➕ Create New Recipe
        var recipe = mapper.Map<Recipe>(request);

        recipe.HerbalistId = herbalist.HerbalistId;
        recipe.CreatedDate = DateTime.UtcNow;
        recipe.IsActive = true;

        recipe.RecipeHerbs = request.Herbs.Select(h => new RecipeHerb
        {
            HerbId = h.HerbId,
            Quantity = h.Quantity
        }).ToList();

        if (request.DiseaseIds != null && request.DiseaseIds.Any())
        {
            recipe.RecipeDiseases = request.DiseaseIds.Select(id => new RecipeDisease
            {
                DiseaseId = id
            }).ToList();
        }

        await unitOfWork.RecipeRepository.CreateAsync(recipe, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var createdRecipe = await unitOfWork.RecipeRepository.GetAsync(
            filter: r => r.RecipeId == recipe.RecipeId,
            tracked: false,
            includeProperties: "RecipeHerbs.Herb,RecipeDiseases.Disease",
            cancellationToken: cancellationToken);

        return mapper.Map<RecipeResponse>(createdRecipe);
    }
    public async Task<IEnumerable<RecipeResponse>> GetAllActiveRecipesAsync(CancellationToken cancellationToken = default)
    {
        var recipes = await unitOfWork.RecipeRepository.GetAllAsync(
            filter: r => r.IsActive,
            tracked: false,
            includeProperties: "RecipeHerbs.Herb,RecipeDiseases.Disease",
            cancellationToken: cancellationToken);

        return mapper.Map<IEnumerable<RecipeResponse>>(recipes);
    }

    public async Task<RecipeResponse?> GetRecipeByIdAsync(int recipeId, CancellationToken cancellationToken = default)
    {
        var recipe = await unitOfWork.RecipeRepository.GetAsync(
            filter: r => r.RecipeId == recipeId && r.IsActive,
            tracked: false,
            includeProperties: "RecipeHerbs.Herb,RecipeDiseases.Disease",
            cancellationToken: cancellationToken);

        return recipe == null ? null : mapper.Map<RecipeResponse>(recipe);
    }

    public async Task<RecipeResponse?> UpdateRecipeAsync(int userId, int recipeId, UpdateRecipeRequest request, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null) throw new Exception("Herbalist not found.");

        var recipe = await unitOfWork.RecipeRepository.GetAsync(
            filter: r => r.RecipeId == recipeId,
            tracked: true, 
                           
            includeProperties: "RecipeHerbs,RecipeDiseases",
            cancellationToken: cancellationToken);

        if (recipe == null) throw new Exception("Recipe not found.");

        if (recipe.HerbalistId != herbalist.HerbalistId)
            throw new UnauthorizedAccessException("You can only update your own recipes.");

        recipe.Description = request.Description;
        recipe.Instructions = request.Instructions;
        recipe.Price = request.Price;

        recipe.RecipeHerbs.Clear();
        foreach (var herbReq in request.Herbs)
        {
            recipe.RecipeHerbs.Add(new RecipeHerb { HerbId = herbReq.HerbId, Quantity = herbReq.Quantity });
        }

        recipe.RecipeDiseases.Clear();
        if (request.DiseaseIds != null) 
        {
            foreach (var diseaseId in request.DiseaseIds)
            {
                recipe.RecipeDiseases.Add(new RecipeDisease { DiseaseId = diseaseId });
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetRecipeByIdAsync(recipe.RecipeId, cancellationToken);
    }

    public async Task<bool> UpdateRecipeAvailabilityAsync(int userId, int recipeId, CancellationToken cancellationToken = default)
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

    public async Task<IEnumerable<RecipeResponse>> GetRecipesByHerbalistIdAsync(int herbalistId, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        var recipes = await unitOfWork.RecipeRepository.GetAllAsync(
            filter: r => r.HerbalistId == herbalistId && (!isActive.HasValue || r.IsActive == isActive.Value),
            includeProperties: "RecipeHerbs.Herb,RecipeDiseases.Disease",
            tracked: false,
            cancellationToken: cancellationToken);

        return mapper.Map<IEnumerable<RecipeResponse>>(recipes);
    }


}