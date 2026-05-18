using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PhytoIntellect.Application.Contracts.ChatAiRecipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Services;

public class AiChatRecipeService(
    IChatAiPredictionService chatAiPredictionService,
    IUnitOfWork unitOfWork,
    IMapper mapper
    ): IAiChatRecipeService
{
    private readonly IChatAiPredictionService _chatAiPredictionService = chatAiPredictionService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;


    public async Task<AiChatPredictionResult> GenerateChatRecipeAsync(int userId, CreateChatRecipeRequest request, CancellationToken cancellationToken)
    {
        int patientId = await _unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        if (patientId == 0) throw new UnauthorizedAccessException("Patient profile not found.");

        var patient = await _unitOfWork.PatientRepository.GetPatientWithHistoryAsync(patientId);

        if (patient == null)
            throw new Exception("Patient not found.");


        var predictionResult = await _chatAiPredictionService.GetChatPredictionAsync(request.UserPrompt, cancellationToken);

        var recipeRecord = new AiChatRecipe
        {
            PatientId = patientId,
            UserPrompt = request.UserPrompt,
            RecommendedRecipeName = predictionResult.RecommendedRecipeName,
            MainHerb = predictionResult.MainHerb,  
            Dosage = predictionResult.Dosage,
            MatchPercentage = predictionResult.MatchPercentage,
            Preparation = predictionResult.Preparation,
            ScientificName = predictionResult.ScientificName,
            Contraindications = predictionResult.Contraindications,
            OtherPossibilities = predictionResult.OtherPossibilities ?? [],
            Category = predictionResult.Category,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            IsAvailable = false
        };
        await _unitOfWork.AiChatRecipeRepository.CreateAsync(recipeRecord, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        predictionResult.AiChatRecipeId = recipeRecord.Id;
        return predictionResult;
    }

    public async Task<PaginatedList<AiChatPredictionResult>> GetAllPublicAsync(RequestFilters filters,CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.AiChatRecipeRepository
            .GetQueryable(tracked: false);

        query = query.Where(r => r.IsActive);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();

            query = query.Where(r =>
                r.RecommendedRecipeName.ToLower().Contains(search) ||
                r.MainHerb.ToLower().Contains(search) ||
                r.Category.ToLower().Contains(search));
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";

        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "name" => isDesc
                    ? query.OrderByDescending(r => r.RecommendedRecipeName)
                    : query.OrderBy(r => r.RecommendedRecipeName),

                "matchpercentage" => isDesc
                    ? query.OrderByDescending(r => r.MatchPercentage)
                    : query.OrderBy(r => r.MatchPercentage),

                "date" => isDesc
                    ? query.OrderByDescending(r => r.CreatedAt)
                    : query.OrderBy(r => r.CreatedAt),

                _ => isDesc
                    ? query.OrderByDescending(r => r.CreatedAt)
                    : query.OrderBy(r => r.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(r => r.CreatedAt);
        }

        var projectedQuery = query.ProjectTo<AiChatPredictionResult>(
            _mapper.ConfigurationProvider);

        return await PaginatedList<AiChatPredictionResult>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);
    }

    // ==============================
    // Get Public Recipe By Id
    // ==============================
    public async Task<AiChatPredictionResult> GetPublicByIdAsync(int recipeId,CancellationToken cancellationToken = default)
    {
        var recipe = await _unitOfWork.AiChatRecipeRepository.GetAsync(
            filter: r => r.Id == recipeId && r.IsActive,
            tracked: false,
            cancellationToken: cancellationToken);

        if (recipe == null)
            throw new KeyNotFoundException("AI Chat Recipe not found.");

        return _mapper.Map<AiChatPredictionResult>(recipe);
    }

    // ==============================
    // Get All Patient Recipes
    // ==============================
    public async Task<PaginatedList<AiChatPredictionResult>> GetPatientAllAsync(int userId,RequestFilters filters,
        CancellationToken cancellationToken = default)
    {
        int patientId = await _unitOfWork.PatientRepository
            .GetIdByUserIdAsync(userId.ToString());

        if (patientId == 0)
            throw new UnauthorizedAccessException("Patient profile not found.");

        var query = _unitOfWork.AiChatRecipeRepository
            .GetQueryable(tracked: false);

        query = query.Where(r => r.PatientId == patientId);

        // 🔍 Search
        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();

            query = query.Where(r =>
                r.RecommendedRecipeName.ToLower().Contains(search) ||
                r.MainHerb.ToLower().Contains(search) ||
                r.Category.ToLower().Contains(search));
        }

        // 🔃 Sorting
        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";

        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "name" => isDesc
                    ? query.OrderByDescending(r => r.RecommendedRecipeName)
                    : query.OrderBy(r => r.RecommendedRecipeName),

                "matchpercentage" => isDesc
                    ? query.OrderByDescending(r => r.MatchPercentage)
                    : query.OrderBy(r => r.MatchPercentage),

                "date" => isDesc
                    ? query.OrderByDescending(r => r.CreatedAt)
                    : query.OrderBy(r => r.CreatedAt),

                _ => isDesc
                    ? query.OrderByDescending(r => r.CreatedAt)
                    : query.OrderBy(r => r.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(r => r.CreatedAt);
        }

        var projectedQuery = query.ProjectTo<AiChatPredictionResult>(
            _mapper.ConfigurationProvider);

        return await PaginatedList<AiChatPredictionResult>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);
    }
    // ============================== 
    // Get Patient Recipe By Id
    // ==============================
    public async Task<AiChatPredictionResult> GetPatientRecipeByIdAsync(int userId,int recipeId,CancellationToken cancellationToken = default)
    {
        int patientId = await _unitOfWork.PatientRepository
            .GetIdByUserIdAsync(userId.ToString());

        if (patientId == 0)
            throw new UnauthorizedAccessException("Patient profile not found.");

        var recipe = await _unitOfWork.AiChatRecipeRepository.GetAsync(
            filter: r => r.Id == recipeId && r.PatientId == patientId,
            tracked: false,
            cancellationToken: cancellationToken);

        if (recipe == null)
            throw new UnauthorizedAccessException("Recipe not found or access denied.");

        return _mapper.Map<AiChatPredictionResult>(recipe);
    }


    // ==============================
    // [Admin] Get All System Consultations
    // ==============================
    public async Task<PaginatedList<AiChatPredictionResult>> GetAllForAdminAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.AiChatRecipeRepository.GetQueryable(tracked: false);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();

            query = query.Where(r =>
                r.RecommendedRecipeName.ToLower().Contains(search) ||
                r.MainHerb.ToLower().Contains(search) ||
                r.Category.ToLower().Contains(search) ||
                r.UserPrompt.ToLower().Contains(search)); 
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";

        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "name" => isDesc ? query.OrderByDescending(r => r.RecommendedRecipeName) : query.OrderBy(r => r.RecommendedRecipeName),
                "matchpercentage" => isDesc ? query.OrderByDescending(r => r.MatchPercentage) : query.OrderBy(r => r.MatchPercentage),
                "date" => isDesc ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt),
                _ => isDesc ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(r => r.CreatedAt);
        }

        var projectedQuery = query.ProjectTo<AiChatPredictionResult>(_mapper.ConfigurationProvider);

        return await PaginatedList<AiChatPredictionResult>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);
    }

    // ==============================
    // [Admin] Toggle Recipe Active Status
    // ==============================
    public async Task<bool> ToggleActiveStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var recipe = await _unitOfWork.AiChatRecipeRepository.GetAsync(
            filter: r => r.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (recipe == null) return false;

        recipe.IsActive = !recipe.IsActive;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    // ==============================
    // [Admin] Get Dashboard Statistics
    // ==============================
    public async Task<object> GetAdminStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.AiChatRecipeRepository.GetQueryable(tracked: false);

        var totalRecipes = await query.CountAsync(cancellationToken);
        var activeRecipes = await query.CountAsync(r => r.IsActive, cancellationToken);
        var inactiveRecipes = totalRecipes - activeRecipes;

        var topCategory = await query
            .GroupBy(r => r.Category)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefaultAsync(cancellationToken) ?? "N/A";

        return new
        {
            TotalAiConsultations = totalRecipes,
            ActiveConsultations = activeRecipes,
            BlockedConsultations = inactiveRecipes,
            MostRequestedCategory = topCategory
        };
    }

}