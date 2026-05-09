using AutoMapper;
using AutoMapper.QueryableExtensions;
using PhytoIntellect.Application.Contracts.Recipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
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

        var isSameRecipe = existingRecipes.Any(r =>
            r.Instructions!.Trim().ToLower() == normalizedInstructions &&

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

        if (isSameRecipe)
        {
            throw new Exception("Recipe already exists. or you can make update for price or description.");
        }

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
    public async Task<PaginatedList<RecipeResponse>> GetAllActiveRecipesAsync(RequestFilters filters,
    CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.RecipeRepository
            .GetQueryable(tracked: false)
            .Where(r => r.IsActive);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(r => r.RecipeDiseases.Any(rd => rd.Disease.DiseaseName.ToLower().Contains(search)));
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";

        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "averagerating" => isDesc ? query.OrderByDescending(r => r.AverageRating) : query.OrderBy(r => r.AverageRating),
                "createddate" => isDesc ? query.OrderByDescending(r => r.CreatedDate) : query.OrderBy(r => r.CreatedDate),
                "price" => isDesc ? query.OrderByDescending(r => r.Price) : query.OrderBy(r => r.Price),
                _ => isDesc ? query.OrderByDescending(r => r.AverageRating) : query.OrderBy(r => r.AverageRating)
            };
        }
        else
        {
            query = isDesc ? query.OrderByDescending(r => r.AverageRating) : query.OrderBy(r => r.AverageRating);
        }

        var projectedQuery = query.ProjectTo<RecipeResponse>(mapper.ConfigurationProvider);

        var result = await PaginatedList<RecipeResponse>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);

        return result;
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

    public async Task<bool?> ToggleRecipeAvailabilityAsync(int userId, int recipeId, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            filter: h => h.UserId == userId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (herbalist == null) return null;

        var recipe = await unitOfWork.RecipeRepository.GetAsync(
            filter: r => r.RecipeId == recipeId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (recipe == null || recipe.HerbalistId != herbalist.HerbalistId) return null;

        recipe.IsActive = !recipe.IsActive;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return recipe.IsActive;
    }

    public async Task<PaginatedList<RecipeResponse>> GetRecipesByHerbalistIdAsync(int herbalistId,RequestFilters filters,bool? isActive = null,CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.RecipeRepository
            .GetQueryable(tracked: false)
            .Where(r => r.HerbalistId == herbalistId);

        // 🔥 optional filter
        if (isActive.HasValue)
        {
            query = query.Where(r => r.IsActive == isActive.Value);
        }

        // 🔍 Search
        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();

            query = query.Where(r =>
                r.Description.ToLower().Contains(search));
        }

        // 🔃 Sorting
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = filters.SortDirection?.ToUpper() == "DESC";

            query = filters.SortColumn.ToLower() switch
            {
                "name" => isDesc
                    ? query.OrderByDescending(r => r.Description)
                    : query.OrderBy(r => r.Description),

                _ => query.OrderBy(r => r.RecipeId)
            };
        }
        else
        {
            query = query.OrderBy(r => r.RecipeId);
        }

        // 🚀 Projection
        var projectedQuery = query.ProjectTo<RecipeResponse>(
            mapper.ConfigurationProvider);

        // 📄 Pagination
        var result = await PaginatedList<RecipeResponse>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);

        return result;
    }


}