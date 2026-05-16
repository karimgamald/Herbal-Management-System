using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PhytoIntellect.Application.Contracts.Reviews;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Services;

public class ReviewRecipeService(IUnitOfWork unitOfWork, IMapper mapper) : IReviewRecipeService
{
    public async Task<ReviewResponse> SubmitReviewAsync(int userId, int aiRecipeId, SubmitReviewRequest request, CancellationToken cancellationToken = default)
    {
        float cleanRating = (float)Math.Round(request.RatingValue, 1);
        if (cleanRating < 1 || cleanRating > 5)
            throw new Exception("Rating must be between 1 and 5.");

        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == userId, tracked: false, includeProperties: "User", cancellationToken: cancellationToken);
        if (herbalist == null)
            throw new Exception("Herbalist not found.");

        // 👈 2. بنكلم جدول الـ AiRecipe بدل الـ Recipe العادي
        var aiRecipe = await unitOfWork.AiRecipeRepository.GetAsync(
            r => r.Id == aiRecipeId, // تأكد إن الـ Primary Key اسمه Id في كلاس AiRecipe
            tracked: true,
            cancellationToken: cancellationToken);

        if (aiRecipe == null)
            throw new Exception("AI Recipe not found.");

        // ❌ شلنا الـ Validations القديمة بتاعة (IsActive) و (Your own recipe) لأن الـ AI ملوش مالك ومش بيحتاج تفعيل

        // 👈 3. بندور في جدول المراجعات باستخدام AiRecipeId
        var existingReview = await unitOfWork.ReviewRecipeRepository.GetAsync(
            r => r.AiRecipeId == aiRecipeId && r.HerbalistId == herbalist.HerbalistId, tracked: true, cancellationToken: cancellationToken);

        ReviewRecipe reviewEntity;

        if (existingReview != null)
        {
            float oldRating = existingReview.RatingValue;
            existingReview.RatingValue = cleanRating;
            existingReview.Comment = request.Comment;
            existingReview.RatingDate = DateTime.UtcNow;

            // 👈 4. بنحسب المتوسط على عواميد الـ AiRecipe الجديدة
            var calculatedAverage = ((aiRecipe.HerbalistAverageRating * aiRecipe.HerbalistTotalRatings) - oldRating + cleanRating) / aiRecipe.HerbalistTotalRatings;
            aiRecipe.HerbalistAverageRating = (float)Math.Round(calculatedAverage, 1);

            reviewEntity = existingReview;
        }
        else
        {
            reviewEntity = new ReviewRecipe
            {
                AiRecipeId = aiRecipeId, // 👈 استخدمنا AiRecipeId
                HerbalistId = herbalist.HerbalistId,
                RatingValue = cleanRating,
                Comment = request.Comment,
                RatingDate = DateTime.UtcNow
            };

            await unitOfWork.ReviewRecipeRepository.CreateAsync(reviewEntity, cancellationToken);

            // 👈 5. بنحسب المتوسط الجديد
            var calculatedAverage = ((aiRecipe.HerbalistAverageRating * aiRecipe.HerbalistTotalRatings) + cleanRating) / (aiRecipe.HerbalistTotalRatings + 1);
            aiRecipe.HerbalistAverageRating = (float)Math.Round(calculatedAverage, 1);
            aiRecipe.HerbalistTotalRatings += 1;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<ReviewResponse>(reviewEntity);
        return response with { HerbalistName = herbalist.User?.FullName ?? "Unknown Herbalist" };
    }

    public async Task<PaginatedList<ReviewResponse>> GetAllRecipeReviewsAsync(
    int aiRecipeId,
    RequestFilters filters,
    CancellationToken cancellationToken = default)
    {
        var aiRecipeExists = await unitOfWork.AiRecipeRepository.GetAsync(r => r.Id == aiRecipeId, tracked: false,
            cancellationToken: cancellationToken);

        if (aiRecipeExists == null)
            return new PaginatedList<ReviewResponse>(
                new List<ReviewResponse>(),
                0,
                filters.PageNumber,
                filters.PageSize);

        // 🔥 Query + Filter by Recipe (مهم جدًا)
        var query = unitOfWork.ReviewRecipeRepository
            .GetQueryable(tracked: false)
            .Where(r => r.AiRecipeId == aiRecipeId);

        // 🔍 Search
        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue;

            query = query.Where(r =>
                (r.Comment != null && EF.Functions.Like(r.Comment, $"%{search}%")) ||
                EF.Functions.Like(r.Herbalist!.User!.FullName, $"%{search}%"));
        }

        // 🔃 Sorting
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = string.Equals(
                filters.SortDirection,
                "DESC",
                StringComparison.OrdinalIgnoreCase);

            query = filters.SortColumn.ToLower() switch
            {
                "rating" => isDesc ? query.OrderByDescending(r => r.RatingValue) : query.OrderBy(r => r.RatingValue),

                "date" => isDesc ? query.OrderByDescending(r => r.RatingDate) : query.OrderBy(r => r.RatingDate),

                "herbalistname" => isDesc ? query.OrderByDescending(r => r.Herbalist!.User!.FullName) : query.OrderBy(r => r.Herbalist.User.FullName),

                _ => isDesc ? query.OrderByDescending(r => r.RatingValue) : query.OrderBy(r => r.RatingValue)
            };
        }
        else
        {
            // ✅ Default sorting = top rated
            query = query.OrderByDescending(r => r.RatingValue);
        }

        // 🚀 Projection
        var projectedQuery = query.ProjectTo<ReviewResponse>(
            mapper.ConfigurationProvider);

        // 📄 Pagination
        var result = await PaginatedList<ReviewResponse>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);

        return result;
    }

    public async Task<ReviewResponse?> GetMyReviewAsync(int userId, int aiRecipeId, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == userId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null) return null;

        var review = await unitOfWork.ReviewRecipeRepository.GetAsync(
            filter: r => r.AiRecipeId == aiRecipeId && r.HerbalistId == herbalist.HerbalistId, // 👈 AiRecipeId
            tracked: false,
            includeProperties: "Herbalist.User",
            cancellationToken: cancellationToken);

        if (review == null) return null;

        return mapper.Map<ReviewResponse>(review);
    }

    public async Task<bool> DeleteMyReviewAsync(int userId, int aiRecipeId, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == userId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null) return false;

        var review = await unitOfWork.ReviewRecipeRepository.GetAsync(
            filter: r => r.AiRecipeId == aiRecipeId && r.HerbalistId == herbalist.HerbalistId, // 👈 AiRecipeId
            tracked: true,
            cancellationToken: cancellationToken);

        if (review == null) return false;

        var aiRecipe = await unitOfWork.AiRecipeRepository.GetAsync(r => r.Id == aiRecipeId, tracked: true, cancellationToken: cancellationToken);

        if (aiRecipe != null)
        {
            if (aiRecipe.HerbalistTotalRatings == 1)
            {
                aiRecipe.HerbalistAverageRating = 0;
                aiRecipe.HerbalistTotalRatings = 0;
            }
            else
            {
                var calculatedAverage = ((aiRecipe.HerbalistAverageRating * aiRecipe.HerbalistTotalRatings) - review.RatingValue) / (aiRecipe.HerbalistTotalRatings - 1);
                aiRecipe.HerbalistAverageRating = (float)Math.Round(calculatedAverage, 1);
                aiRecipe.HerbalistTotalRatings -= 1;
            }
        }

        unitOfWork.ReviewRecipeRepository.Remove(review);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    // ==========================================
    // (AiChatRecipeId) - NEW Ecosystem
    // ==========================================

    public async Task<ReviewResponse> SubmitAiChatReviewAsync(int userId, int aiChatRecipeId, SubmitReviewRequest request, CancellationToken cancellationToken = default)
    {
        float cleanRating = (float)Math.Round(request.RatingValue, 1);
        if (cleanRating < 1 || cleanRating > 5)
            throw new Exception("Rating must be between 1 and 5.");

        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == userId, tracked: false, includeProperties: "User", cancellationToken: cancellationToken);
        if (herbalist == null)
            throw new Exception("Herbalist not found.");

        var aiChatRecipe = await unitOfWork.AiChatRecipeRepository.GetAsync(
            r => r.Id == aiChatRecipeId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (aiChatRecipe == null)
            throw new Exception("AI Chat Recipe not found.");

        var existingReview = await unitOfWork.ReviewRecipeRepository.GetAsync(
            r => r.AiChatRecipeId == aiChatRecipeId && r.HerbalistId == herbalist.HerbalistId, tracked: true, cancellationToken: cancellationToken);

        ReviewRecipe reviewEntity;

        if (existingReview != null)
        {
            float oldRating = existingReview.RatingValue;
            existingReview.RatingValue = cleanRating;
            existingReview.Comment = request.Comment;
            existingReview.RatingDate = DateTime.UtcNow;

            var calculatedAverage = ((aiChatRecipe.HerbalistAverageRating * aiChatRecipe.HerbalistTotalRatings) - oldRating + cleanRating) / aiChatRecipe.HerbalistTotalRatings;
            aiChatRecipe.HerbalistAverageRating = (float)Math.Round(calculatedAverage, 1);

            reviewEntity = existingReview;
        }
        else
        {
            reviewEntity = new ReviewRecipe
            {
                AiRecipeId = null,
                AiChatRecipeId = aiChatRecipeId,
                HerbalistId = herbalist.HerbalistId,
                RatingValue = cleanRating,
                Comment = request.Comment,
                RatingDate = DateTime.UtcNow
            };

            await unitOfWork.ReviewRecipeRepository.CreateAsync(reviewEntity, cancellationToken);

            var calculatedAverage = ((aiChatRecipe.HerbalistAverageRating * aiChatRecipe.HerbalistTotalRatings) + cleanRating) / (aiChatRecipe.HerbalistTotalRatings + 1);
            aiChatRecipe.HerbalistAverageRating = (float)Math.Round(calculatedAverage, 1);
            aiChatRecipe.HerbalistTotalRatings += 1;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<ReviewResponse>(reviewEntity);
        return response with { HerbalistName = herbalist.User?.FullName ?? "Unknown Herbalist" };
    }

    public async Task<PaginatedList<ReviewResponse>> GetAllAiChatRecipeReviewsAsync(
    int aiChatRecipeId,
    RequestFilters filters,
    CancellationToken cancellationToken = default)
    {
        var aiChatRecipeExists = await unitOfWork.AiChatRecipeRepository.GetAsync(r => r.Id == aiChatRecipeId, tracked: false,
            cancellationToken: cancellationToken);

        if (aiChatRecipeExists == null)
            return new PaginatedList<ReviewResponse>(new List<ReviewResponse>(), 0, filters.PageNumber, filters.PageSize);

        var query = unitOfWork.ReviewRecipeRepository
            .GetQueryable(tracked: false)
            .Where(r => r.AiChatRecipeId == aiChatRecipeId); // 👈 الفلتر الجديد

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue;
            query = query.Where(r =>
                (r.Comment != null && EF.Functions.Like(r.Comment, $"%{search}%")) ||
                EF.Functions.Like(r.Herbalist!.User!.FullName, $"%{search}%"));
        }

        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = string.Equals(filters.SortDirection, "DESC", StringComparison.OrdinalIgnoreCase);

            query = filters.SortColumn.ToLower() switch
            {
                "rating" => isDesc ? query.OrderByDescending(r => r.RatingValue) : query.OrderBy(r => r.RatingValue),
                "date" => isDesc ? query.OrderByDescending(r => r.RatingDate) : query.OrderBy(r => r.RatingDate),
                "herbalistname" => isDesc ? query.OrderByDescending(r => r.Herbalist!.User!.FullName) : query.OrderBy(r => r.Herbalist.User.FullName),
                _ => isDesc ? query.OrderByDescending(r => r.ReviewRecipeId) : query.OrderBy(r => r.ReviewRecipeId)
            };
        }
        else
        {
            query = query.OrderByDescending(r => r.RatingValue);
        }

        var projectedQuery = query.ProjectTo<ReviewResponse>(mapper.ConfigurationProvider);
        return await PaginatedList<ReviewResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }

    public async Task<ReviewResponse?> GetMyAiChatReviewAsync(int userId, int aiChatRecipeId, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == userId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null) return null;

        var review = await unitOfWork.ReviewRecipeRepository.GetAsync(
            filter: r => r.AiChatRecipeId == aiChatRecipeId && r.HerbalistId == herbalist.HerbalistId,
            tracked: false,
            includeProperties: "Herbalist.User",
            cancellationToken: cancellationToken);

        if (review == null) return null;

        return mapper.Map<ReviewResponse>(review);
    }

    public async Task<bool> DeleteMyAiChatReviewAsync(int userId, int aiChatRecipeId, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == userId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null) return false;

        var review = await unitOfWork.ReviewRecipeRepository.GetAsync(
            filter: r => r.AiChatRecipeId == aiChatRecipeId && r.HerbalistId == herbalist.HerbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (review == null) return false;

        var aiChatRecipe = await unitOfWork.AiChatRecipeRepository.GetAsync(r => r.Id == aiChatRecipeId, tracked: true, cancellationToken: cancellationToken);

        if (aiChatRecipe != null)
        {
            if (aiChatRecipe.HerbalistTotalRatings == 1)
            {
                aiChatRecipe.HerbalistAverageRating = 0;
                aiChatRecipe.HerbalistTotalRatings = 0;
            }
            else
            {
                var calculatedAverage = ((aiChatRecipe.HerbalistAverageRating * aiChatRecipe.HerbalistTotalRatings) - review.RatingValue) / (aiChatRecipe.HerbalistTotalRatings - 1);
                aiChatRecipe.HerbalistAverageRating = (float)Math.Round(calculatedAverage, 1);
                aiChatRecipe.HerbalistTotalRatings -= 1;
            }
        }

        unitOfWork.ReviewRecipeRepository.Remove(review);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}