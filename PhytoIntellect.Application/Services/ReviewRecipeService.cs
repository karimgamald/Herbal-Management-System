using AutoMapper;
using PhytoIntellect.Application.Contracts.Reviews;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Services;

public class ReviewRecipeService(IUnitOfWork unitOfWork, IMapper mapper) : IReviewRecipeService
{
    public async Task<ReviewResponse> SubmitReviewAsync(int userId, int recipeId, SubmitReviewRequest request, CancellationToken cancellationToken = default)
    {
        // 🛡️ التنظيف والتقريب لرقم عشري واحد
        float cleanRating = (float)Math.Round(request.RatingValue, 1);
        if (cleanRating < 1 || cleanRating > 5) throw new Exception("Rating must be between 1 and 5.");

        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == userId, tracked: false, includeProperties: "User", cancellationToken: cancellationToken);
        if (herbalist == null) throw new Exception("Herbalist not found.");

        // 👈 مش بنسأل عن IsActive عشان يقيم وصفات الـ AI
        var recipe = await unitOfWork.RecipeRepository.GetAsync(r => r.RecipeId == recipeId, tracked: true, cancellationToken: cancellationToken);
        if (recipe == null) throw new Exception("Recipe not found.");

        var existingReview = await unitOfWork.ReviewRecipeRepository.GetAsync(
            r => r.RecipeId == recipeId && r.HerbalistId == herbalist.HerbalistId, tracked: true, cancellationToken: cancellationToken);

        ReviewRecipe reviewEntity;

        if (existingReview != null)
        {
            // 🔄 التعديل (Update)
            float oldRating = existingReview.RatingValue;
            existingReview.RatingValue = cleanRating;
            existingReview.Comment = request.Comment;
            existingReview.RatingDate = DateTime.UtcNow;

            var calculatedAverage = ((recipe.HerbalistAverageRating * recipe.HerbalistTotalRatings) - oldRating + cleanRating) / recipe.HerbalistTotalRatings;
            recipe.HerbalistAverageRating = (float)Math.Round(calculatedAverage, 1); // التقريب لـ 1

            reviewEntity = existingReview;
            // ❌ شيلنا ربط الأوبجيكت هنا عشان الـ EF Core ميتغاباش
        }
        else
        {
            // ➕ الإضافة (Insert)
            reviewEntity = new ReviewRecipe
            {
                RecipeId = recipeId,
                HerbalistId = herbalist.HerbalistId, // 👈 بنباصي الـ ID بس
                RatingValue = cleanRating,
                Comment = request.Comment,
                RatingDate = DateTime.UtcNow
                // ❌ شيلنا ربط الأوبجيكت هنا كمان
            };

            await unitOfWork.ReviewRecipeRepository.CreateAsync(reviewEntity, cancellationToken);

            var calculatedAverage = ((recipe.HerbalistAverageRating * recipe.HerbalistTotalRatings) + cleanRating) / (recipe.HerbalistTotalRatings + 1);
            recipe.HerbalistAverageRating = (float)Math.Round(calculatedAverage, 1); // التقريب لـ 1
            recipe.HerbalistTotalRatings += 1;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 🪄 سحر الـ AutoMapper + إدراج الاسم في الريسبونس عشان إحنا مستخدمين Record
        var response = mapper.Map<ReviewResponse>(reviewEntity);
        return response with { HerbalistName = herbalist.User?.FullName ?? "Unknown Herbalist" };
    }

    public async Task<IEnumerable<ReviewResponse>> GetAllRecipeReviewsAsync(int recipeId, bool isHerbalist, CancellationToken cancellationToken = default)
    {
        var reviews = await unitOfWork.ReviewRecipeRepository.GetAllAsync(
            filter: r => r.RecipeId == recipeId && (isHerbalist || r.Recipe!.IsActive),
            tracked: false,
            includeProperties: "Herbalist.User",
            cancellationToken: cancellationToken);

        // 🪄 سحر الـ AutoMapper
        var mappedReviews = mapper.Map<IEnumerable<ReviewResponse>>(reviews);
        return mappedReviews.OrderByDescending(r => r.RatingDate).ToList();
    }

    public async Task<ReviewResponse?> GetMyReviewAsync(int userId, int recipeId, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == userId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null) return null;

        var review = await unitOfWork.ReviewRecipeRepository.GetAsync(
            filter: r => r.RecipeId == recipeId && r.HerbalistId == herbalist.HerbalistId,
            tracked: false,
            includeProperties: "Herbalist.User",
            cancellationToken: cancellationToken);

        if (review == null) return null;

        // 🪄 سحر الـ AutoMapper
        return mapper.Map<ReviewResponse>(review);
    }

    public async Task<bool> DeleteMyReviewAsync(int userId, int recipeId, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(h => h.UserId == userId, tracked: false, cancellationToken: cancellationToken);
        if (herbalist == null) return false;

        var review = await unitOfWork.ReviewRecipeRepository.GetAsync(
            filter: r => r.RecipeId == recipeId && r.HerbalistId == herbalist.HerbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (review == null) return false;

        var recipe = await unitOfWork.RecipeRepository.GetAsync(r => r.RecipeId == recipeId, tracked: true, cancellationToken: cancellationToken);
        if (recipe != null)
        {
            if (recipe.HerbalistTotalRatings == 1)
            {
                recipe.HerbalistAverageRating = 0;
                recipe.HerbalistTotalRatings = 0;
            }
            else
            {
                var calculatedAverage = ((recipe.HerbalistAverageRating * recipe.HerbalistTotalRatings) - review.RatingValue) / (recipe.HerbalistTotalRatings - 1);
                recipe.HerbalistAverageRating = (float)Math.Round(calculatedAverage, 1); // التقريب لـ 1
                recipe.HerbalistTotalRatings -= 1;
            }
        }

        unitOfWork.ReviewRecipeRepository.Remove(review);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}