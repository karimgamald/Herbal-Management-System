using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PhytoIntellect.Application.Contracts.Feedbacks;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Application.Services;

public class FeedbackService(IUnitOfWork unitOfWork, IMapper mapper) : IFeedbackService
{
    public async Task<FeedbackResponse> SubmitRecipeFeedbackAsync(int userId, int recipeId, SubmitFeedbackRequest request, CancellationToken cancellationToken = default)
    {
        int patientId = await unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        if (patientId == 0) throw new Exception("Patient not found.");

        var recipe = await unitOfWork.RecipeRepository.GetAsync(r => r.RecipeId == recipeId && r.IsActive, tracked: true, cancellationToken: cancellationToken);
        if (recipe == null) throw new Exception("Recipe not found.");

        var existingFeedback = await unitOfWork.FeedbackRepository.GetAsync(f => f.RecipeId == recipeId && f.PatientId == patientId, tracked: true, cancellationToken: cancellationToken);

        Feedback feedbackEntity;
        float cleanRating = (float)Math.Round(request.RatingValue, 1);

        if (existingFeedback != null)
        {
            float oldRating = existingFeedback.RatingValue;
            existingFeedback.RatingValue = cleanRating;
            existingFeedback.Comment = request.Comment;
            existingFeedback.RatingDate = DateTime.UtcNow;

            var calc = ((recipe.AverageRating * recipe.TotalRatings) - oldRating + cleanRating) / recipe.TotalRatings;
            recipe.AverageRating = (float)Math.Round(calc, 1);
            feedbackEntity = existingFeedback;
        }
        else
        {
            feedbackEntity = new Feedback
            {
                RecipeId = recipeId,
                AiRecipeId = null, 
                PatientId = patientId,
                RatingValue = cleanRating,
                Comment = request.Comment,
                RatingDate = DateTime.UtcNow
            };
            await unitOfWork.FeedbackRepository.CreateAsync(feedbackEntity, cancellationToken);

            var calc = ((recipe.AverageRating * recipe.TotalRatings) + cleanRating) / (recipe.TotalRatings + 1);
            recipe.AverageRating = (float)Math.Round(calc, 1);
            recipe.TotalRatings += 1;
        }

        #region

        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
         filter: h => h.HerbalistId == recipe.HerbalistId,
         tracked: true,
         cancellationToken: cancellationToken);

        if (herbalist != null)
        {
            // التريكة هنا: بنجيب كل الوصفات "ما عدا" الوصفة اللي بنقيمها دلوقتي
            var otherRecipes = await unitOfWork.RecipeRepository.GetAllAsync(
                filter: r => r.HerbalistId == recipe.HerbalistId && r.RecipeId != recipeId && r.TotalRatings > 0,
                tracked: false,
                cancellationToken: cancellationToken);

            // نحسب نقط باقي الوصفات
            float otherPoints = otherRecipes.Sum(r => r.AverageRating * r.TotalRatings);
            int otherVotes = otherRecipes.Sum(r => r.TotalRatings);

            // نجمع عليهم نقط الوصفة الحالية اللي اتعدلت فوق
            float totalPoints = otherPoints + (recipe.AverageRating * recipe.TotalRatings);
            int totalVotes = otherVotes + recipe.TotalRatings;

            if (totalVotes > 0)
            {
                var calc = totalPoints / totalVotes;
                herbalist.AverageRating = (float)Math.Round(calc, 1);
            }
            else
            {
                herbalist.AverageRating = 0;
            }
        }

        #endregion


        await unitOfWork.SaveChangesAsync(cancellationToken);

        var feedback = await unitOfWork.FeedbackRepository
            .GetQueryable()
            .Where(f => f.FeedbackId == feedbackEntity.FeedbackId)
            .Include(f => f.Patient)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(cancellationToken);

        return new FeedbackResponse
        {
            FeedbackId = feedback.FeedbackId,

            // ✅ IDs (كل الحالات)
            RecipeId = feedback.RecipeId,
            AiRecipeId = feedback.AiRecipeId,
            AiChatRecipeId = feedback.AiChatRecipeId,

            // ⭐ Data
            RatingValue = feedback.RatingValue,
            Comment = feedback.Comment,
            RatingDate = feedback.RatingDate,

            // 👤 Patient Name
            PatientName = feedback.Patient?.User?.FullName ?? "Unknown"
        };
    }

    public async Task<PaginatedList<FeedbackResponse>> GetRecipeFeedbacksAsync(
     int recipeId,
     RequestFilters filters,
     CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.FeedbackRepository.GetQueryable(tracked: false);

        query = query.Where(f => f.RecipeId == recipeId);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(f => f.Patient!.User!.FullName.ToLower().Contains(search));
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "ratingvalue" => isDesc ? query.OrderByDescending(f => f.RatingValue) : query.OrderBy(f => f.RatingValue),
                "date" => isDesc ? query.OrderByDescending(f => f.RatingDate) : query.OrderBy(f => f.RatingDate),
                "patientname" => isDesc ? query.OrderByDescending(f => f.Patient!.User!.FullName) : query.OrderBy(f => f.Patient!.User!.FullName),
                _ => isDesc ? query.OrderByDescending(f => f.RatingValue) : query.OrderBy(f => f.RatingValue)
            };
        }
        else
        {
            query = isDesc ? query.OrderByDescending(f => f.RatingValue) : query.OrderBy(f => f.RatingValue);
        }

        var projectedQuery = query.ProjectTo<FeedbackResponse>(mapper.ConfigurationProvider);

        return await PaginatedList<FeedbackResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }

    public async Task<FeedbackResponse?> GetMyRecipeFeedbackAsync(int userId, int recipeId, CancellationToken cancellationToken = default)
    {
        int patientId = await unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        var feedback = await unitOfWork.FeedbackRepository.GetAsync(f => f.RecipeId == recipeId && f.PatientId == patientId, tracked: false, includeProperties: "Patient.User", cancellationToken: cancellationToken);
        return feedback == null ? null : mapper.Map<FeedbackResponse>(feedback);
    }

    public async Task<bool> DeleteMyRecipeFeedbackAsync(int userId, int recipeId, CancellationToken cancellationToken = default)
    {
        int patientId = await unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        var feedback = await unitOfWork.FeedbackRepository.GetAsync(f => f.RecipeId == recipeId && f.PatientId == patientId, tracked: true, cancellationToken: cancellationToken);
        if (feedback == null) return false;

        var recipe = await unitOfWork.RecipeRepository.GetAsync(r => r.RecipeId == recipeId, tracked: true, cancellationToken: cancellationToken);
        if (recipe != null)
        {
            if (recipe.TotalRatings == 1) { recipe.AverageRating = 0; recipe.TotalRatings = 0; }
            else
            {
                var calc = ((recipe.AverageRating * recipe.TotalRatings) - feedback.RatingValue) / (recipe.TotalRatings - 1);
                recipe.AverageRating = (float)Math.Round(calc, 1);
                recipe.TotalRatings -= 1;
            }
        }
        unitOfWork.FeedbackRepository.Remove(feedback);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    // (AiRecipeId) 
    public async Task<FeedbackResponse> SubmitAiRecipeFeedbackAsync(int userId, int aiRecipeId, SubmitFeedbackRequest request, CancellationToken cancellationToken = default)
    {
        int patientId = await unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        if (patientId == 0) throw new UnauthorizedAccessException("Patient not found.");

        var aiRecipe = await unitOfWork.AiRecipeRepository.GetAsync(r => r.Id == aiRecipeId, tracked: true, cancellationToken: cancellationToken);
        if (aiRecipe == null) throw new KeyNotFoundException("AI Recipe not found.");

        if (aiRecipe.PatientId != patientId)
            throw new UnauthorizedAccessException("You are not authorized to evaluate an AI prescription for another patient.");

        var existingFeedback = await unitOfWork.FeedbackRepository.GetAsync(f => f.AiRecipeId == aiRecipeId && f.PatientId == patientId, tracked: true, cancellationToken: cancellationToken);

        Feedback feedbackEntity;
        float cleanRating = (float)Math.Round(request.RatingValue, 1);

        if (existingFeedback != null)
        {
            existingFeedback.RatingValue = cleanRating;
            existingFeedback.Comment = request.Comment;
            existingFeedback.RatingDate = DateTime.UtcNow;

            aiRecipe.Rating = cleanRating;
            feedbackEntity = existingFeedback;
        }
        else
        {
            feedbackEntity = new Feedback
            {
                RecipeId = null,
                AiRecipeId = aiRecipeId,
                PatientId = patientId,
                RatingValue = cleanRating,
                Comment = request.Comment,
                RatingDate = DateTime.UtcNow
            };
            await unitOfWork.FeedbackRepository.CreateAsync(feedbackEntity, cancellationToken);

            aiRecipe.Rating = cleanRating;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var feedback = await unitOfWork.FeedbackRepository
            .GetQueryable()
            .Where(f => f.FeedbackId == feedbackEntity.FeedbackId)
            .Include(f => f.Patient)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(cancellationToken);

        return new FeedbackResponse
        {
            FeedbackId = feedback.FeedbackId,

            // ✅ IDs (كل الحالات)
            RecipeId = feedback.RecipeId,
            AiRecipeId = feedback.AiRecipeId,
            AiChatRecipeId = feedback.AiChatRecipeId,

            // ⭐ Data
            RatingValue = feedback.RatingValue,
            Comment = feedback.Comment,
            RatingDate = feedback.RatingDate,

            // 👤 Patient Name
            PatientName = feedback.Patient?.User?.FullName ?? "Unknown"
        };
    }

    public async Task<PaginatedList<FeedbackResponse>> GetAiRecipeFeedbacksAsync(
    int aiRecipeId,
    RequestFilters filters,
    CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.FeedbackRepository.GetQueryable(tracked: false);

        query = query.Where(f => f.AiRecipeId == aiRecipeId);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(f => f.Patient!.User!.FullName.ToLower().Contains(search));
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "ratingvalue" => isDesc ? query.OrderByDescending(f => f.RatingValue) : query.OrderBy(f => f.RatingValue),
                "date" => isDesc ? query.OrderByDescending(f => f.RatingDate) : query.OrderBy(f => f.RatingDate),
                "patientname" => isDesc ? query.OrderByDescending(f => f.Patient!.User!.FullName) : query.OrderBy(f => f.Patient!.User!.FullName),
                _ => isDesc ? query.OrderByDescending(f => f.RatingValue) : query.OrderBy(f => f.RatingValue)
            };
        }
        else
        {
            query = isDesc ? query.OrderByDescending(f => f.RatingValue) : query.OrderBy(f => f.RatingValue);
        }

        var projectedQuery = query.ProjectTo<FeedbackResponse>(mapper.ConfigurationProvider);

        return await PaginatedList<FeedbackResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }

    public async Task<FeedbackResponse?> GetMyAiRecipeFeedbackAsync(int userId, int aiRecipeId, CancellationToken cancellationToken = default)
    {
        int patientId = await unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        var feedback = await unitOfWork.FeedbackRepository.GetAsync(f => f.AiRecipeId == aiRecipeId && f.PatientId == patientId, tracked: false, includeProperties: "Patient.User", cancellationToken: cancellationToken);
        return feedback == null ? null : mapper.Map<FeedbackResponse>(feedback);
    }

    public async Task<bool> DeleteMyAiRecipeFeedbackAsync(int userId, int aiRecipeId, CancellationToken cancellationToken = default)
    {
        int patientId = await unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        if (patientId == 0) throw new UnauthorizedAccessException("Patient not found.");

        var feedback = await unitOfWork.FeedbackRepository.GetAsync(
            f => f.AiRecipeId == aiRecipeId && f.PatientId == patientId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (feedback == null) return false;

        var aiRecipe = await unitOfWork.AiRecipeRepository.GetAsync(
            r => r.Id == aiRecipeId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (aiRecipe != null)
        {
            aiRecipe.Rating = null;
        }

        unitOfWork.FeedbackRepository.Remove(feedback);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }


    // ==========================================
    // (AiChatRecipeId) - NEW Ecosytem
    // ==========================================

    public async Task<FeedbackResponse> SubmitAiChatRecipeFeedbackAsync(int userId, int aiChatRecipeId, SubmitFeedbackRequest request, CancellationToken cancellationToken = default)
    {
        int patientId = await unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        if (patientId == 0) throw new UnauthorizedAccessException("Patient not found.");

        var aiChatRecipe = await unitOfWork.AiChatRecipeRepository.GetAsync(r => r.Id == aiChatRecipeId, tracked: true, cancellationToken: cancellationToken);
        if (aiChatRecipe == null) throw new KeyNotFoundException("AI Chat Recipe not found.");

        if (aiChatRecipe.PatientId != patientId)
            throw new UnauthorizedAccessException("You are not authorized to evaluate an AI Chat prescription for another patient.");

        var existingFeedback = await unitOfWork.FeedbackRepository.GetAsync(f => f.AiChatRecipeId == aiChatRecipeId && f.PatientId == patientId, tracked: true, cancellationToken: cancellationToken);

        Feedback feedbackEntity;
        float cleanRating = (float)Math.Round(request.RatingValue, 1);

        if (existingFeedback != null)
        {
            existingFeedback.RatingValue = cleanRating;
            existingFeedback.Comment = request.Comment;
            existingFeedback.RatingDate = DateTime.UtcNow;

            aiChatRecipe.Rating = cleanRating; // Update the chat rating directly
            feedbackEntity = existingFeedback;
        }
        else
        {
            feedbackEntity = new Feedback
            {
                RecipeId = null,
                AiRecipeId = null,
                AiChatRecipeId = aiChatRecipeId, // 👈 The new field
                PatientId = patientId,
                RatingValue = cleanRating,
                Comment = request.Comment,
                RatingDate = DateTime.UtcNow
            };
            await unitOfWork.FeedbackRepository.CreateAsync(feedbackEntity, cancellationToken);

            aiChatRecipe.Rating = cleanRating;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var feedback = await unitOfWork.FeedbackRepository
            .GetQueryable()
            .Where(f => f.FeedbackId == feedbackEntity.FeedbackId)
            .Include(f => f.Patient)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(cancellationToken);

        return new FeedbackResponse
        {
            FeedbackId = feedback.FeedbackId,

            // ✅ IDs (كل الحالات)
            RecipeId = feedback.RecipeId,
            AiRecipeId = feedback.AiRecipeId,
            AiChatRecipeId = feedback.AiChatRecipeId,

            // ⭐ Data
            RatingValue = feedback.RatingValue,
            Comment = feedback.Comment,
            RatingDate = feedback.RatingDate,

            // 👤 Patient Name
            PatientName = feedback.Patient?.User?.FullName ?? "Unknown"
        };
    }

    public async Task<PaginatedList<FeedbackResponse>> GetAiChatRecipeFeedbacksAsync(
    int aiChatRecipeId,
    RequestFilters filters,
    CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.FeedbackRepository.GetQueryable(tracked: false);

        query = query.Where(f => f.AiChatRecipeId == aiChatRecipeId);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(f => f.Patient!.User!.FullName.ToLower().Contains(search));
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "ratingvalue" => isDesc ? query.OrderByDescending(f => f.RatingValue) : query.OrderBy(f => f.RatingValue),
                "date" => isDesc ? query.OrderByDescending(f => f.RatingDate) : query.OrderBy(f => f.RatingDate),
                "patientname" => isDesc ? query.OrderByDescending(f => f.Patient!.User!.FullName) : query.OrderBy(f => f.Patient!.User!.FullName),
                _ => isDesc ? query.OrderByDescending(f => f.RatingValue) : query.OrderBy(f => f.RatingValue)
            };
        }
        else
        {
            query = isDesc ? query.OrderByDescending(f => f.RatingValue) : query.OrderBy(f => f.RatingValue);
        }

        var projectedQuery = query.ProjectTo<FeedbackResponse>(mapper.ConfigurationProvider);

        return await PaginatedList<FeedbackResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }

    public async Task<FeedbackResponse?> GetMyAiChatRecipeFeedbackAsync(int userId, int aiChatRecipeId, CancellationToken cancellationToken = default)
    {
        int patientId = await unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        var feedback = await unitOfWork.FeedbackRepository.GetAsync(f => f.AiChatRecipeId == aiChatRecipeId && f.PatientId == patientId, tracked: false, includeProperties: "Patient.User", cancellationToken: cancellationToken);
        return feedback == null ? null : mapper.Map<FeedbackResponse>(feedback);
    }

    public async Task<bool> DeleteMyAiChatRecipeFeedbackAsync(int userId, int aiChatRecipeId, CancellationToken cancellationToken = default)
    {
        int patientId = await unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());
        if (patientId == 0) throw new UnauthorizedAccessException("Patient not found.");

        var feedback = await unitOfWork.FeedbackRepository.GetAsync(
            f => f.AiChatRecipeId == aiChatRecipeId && f.PatientId == patientId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (feedback == null) return false;

        var aiChatRecipe = await unitOfWork.AiChatRecipeRepository.GetAsync(
            r => r.Id == aiChatRecipeId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (aiChatRecipe != null)
        {
            aiChatRecipe.Rating = null; // Clear the rating
        }

        unitOfWork.FeedbackRepository.Remove(feedback);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }



    public async Task<PaginatedList<FeedbackResponse>> GetMyFeedbacksAsync(
    int userId,
    RequestFilters filters,
    CancellationToken cancellationToken = default)
    {
        // 1. نجيب رقم المريض
        int patientId = await unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());

        // لو المريض مش موجود بنرجع كرتونة Pagination فاضية
        if (patientId == 0)
            return new PaginatedList<FeedbackResponse>(new List<FeedbackResponse>(), filters.PageNumber, 0, filters.PageSize);

        // 2. نجيب الـ IQueryable
        var query = unitOfWork.FeedbackRepository.GetQueryable(tracked: false);

        // الفلتر الأساسي (كل التقييمات بتاعة المريض ده)
        query = query.Where(f => f.PatientId == patientId);

        // 3. البحث
        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(f => f.Comment.ToLower().Contains(search));
        }

        // 4. الترتيب
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
            query = filters.SortColumn.ToLower() switch
            {
                "ratingValue" => isDesc ? query.OrderByDescending(f => f.RatingValue) : query.OrderBy(f => f.RatingValue),
                "date" => isDesc ? query.OrderByDescending(f => f.RatingDate) : query.OrderBy(f => f.RatingDate),
                _ => query.OrderByDescending(f => f.RatingDate)
            };
        }
        else
        {
            query = query.OrderByDescending(f => f.RatingDate);
        }
         
        // 5. المابينج
        var projectedQuery = query.ProjectTo<FeedbackResponse>(mapper.ConfigurationProvider);

        // 6. الـ Pagination
        return await PaginatedList<FeedbackResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }
}