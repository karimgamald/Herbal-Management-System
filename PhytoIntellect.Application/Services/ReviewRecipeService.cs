using AutoMapper;
using PhytoIntellect.Application.Contracts.Reviews;
using PhytoIntellect.Application.Interfaces;
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

    public async Task<IEnumerable<ReviewResponse>> GetAllRecipeReviewsAsync(int aiRecipeId, CancellationToken cancellationToken = default)
    {
        var aiRecipe = await unitOfWork.AiRecipeRepository.GetAsync(r => r.Id == aiRecipeId, tracked: false, cancellationToken: cancellationToken);

        if (aiRecipe == null) return new List<ReviewResponse>();

        var reviews = await unitOfWork.ReviewRecipeRepository.GetAllAsync(
            filter: r => r.AiRecipeId == aiRecipeId, // 👈 فلترة بـ AiRecipeId
            tracked: false,
            includeProperties: "Herbalist.User",
            cancellationToken: cancellationToken);

        var mappedReviews = mapper.Map<IEnumerable<ReviewResponse>>(reviews);
        return mappedReviews.OrderByDescending(r => r.RatingDate).ToList();
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
}