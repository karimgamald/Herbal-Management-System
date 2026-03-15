using AutoMapper;
using PhytoIntellect.Application.Contracts.Feedbacks;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Application.Services;

public class FeedbackService(IUnitOfWork unitOfWork, IMapper mapper) : IFeedbackService
{
    // 1️⃣ إضافة أو تعديل تقييم
    public async Task<FeedbackResponse> SubmitFeedbackAsync(int userId, int recipeId, SubmitFeedbackRequest request, CancellationToken cancellationToken = default)
    {
        

        var patient = await unitOfWork.PatientRepository.GetAsync(
            filter: p => p.UserId == userId,
            tracked: false,
            includeProperties: "User",
            cancellationToken: cancellationToken);

        if (patient == null) throw new Exception("Patient not found.");

        var recipe = await unitOfWork.RecipeRepository.GetAsync(
            filter: r => r.RecipeId == recipeId && r.IsActive,
            tracked: true, // عشان هنعدل التوتال والمتوسط بتاعها
            cancellationToken: cancellationToken);

        if (recipe == null) throw new Exception("Recipe not found.");

        var existingFeedback = await unitOfWork.FeedbackRepository.GetAsync(
            filter: f => f.RecipeId == recipe.RecipeId && f.PatientId == patient.PatientId,
            tracked: true,
            cancellationToken: cancellationToken);

        Feedback feedbackEntity;
        float cleanRating = (float)Math.Round(request.RatingValue, 1);

        if (existingFeedback != null)
        {
            // 🔄 حالة التعديل (Update)
            float oldRating = existingFeedback.RatingValue;

            existingFeedback.RatingValue = cleanRating;
            existingFeedback.Comment = request.Comment;
            existingFeedback.RatingDate = DateTime.UtcNow;

            // 👈 حسبة المتوسط الجديد بعد التعديل (مع التقريب لرقمين عشريين)
            var calculatedAverage = ((recipe.AverageRating * recipe.TotalRatings) - oldRating + request.RatingValue) / recipe.TotalRatings;
            recipe.AverageRating = (float)Math.Round(calculatedAverage, 1);

            feedbackEntity = existingFeedback;
        }
        else
        {
            // ➕ حالة الإضافة (Insert)
            feedbackEntity = new Feedback
            {
                RecipeId = recipe.RecipeId,
                PatientId = patient.PatientId,
                RatingValue = cleanRating,
                Comment = request.Comment,
                RatingDate = DateTime.UtcNow
            };

            await unitOfWork.FeedbackRepository.CreateAsync(feedbackEntity, cancellationToken);

            // 👈 حسبة المتوسط الجديد بعد الإضافة (مع التقريب لرقمين عشريين)
            var calculatedAverage = ((recipe.AverageRating * recipe.TotalRatings) + request.RatingValue) / (recipe.TotalRatings + 1);
            recipe.AverageRating = (float)Math.Round(calculatedAverage, 1);
            recipe.TotalRatings += 1;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new FeedbackResponse
        {
            FeedbackId = feedbackEntity.FeedbackId,
            RecipeId = feedbackEntity.RecipeId,
            RatingValue = feedbackEntity.RatingValue,
            Comment = feedbackEntity.Comment,
            RatingDate = feedbackEntity.RatingDate,
            PatientName = patient.User?.FullName ?? "Unknown Patient"
        };
    }

    // 2️⃣ جلب كل تقييمات وصفة
    public async Task<IEnumerable<FeedbackResponse>> GetRecipeFeedbacksAsync(int recipeId, CancellationToken cancellationToken = default)
    {
        var feedbacks = await unitOfWork.FeedbackRepository.GetAllAsync(
            filter: f => f.RecipeId == recipeId,
            tracked: false,
            includeProperties: "Patient.User",
            cancellationToken: cancellationToken);

        return feedbacks.Select(f => new FeedbackResponse
        {
            FeedbackId = f.FeedbackId,
            RecipeId = f.RecipeId,
            RatingValue = f.RatingValue,
            Comment = f.Comment,
            RatingDate = f.RatingDate,
            PatientName = f.Patient?.User?.FullName ?? "Unknown Patient"
        }).OrderByDescending(f => f.RatingDate).ToList();
    }

    // 3️⃣ جلب تقييم المريض الحالي (لو كان مقيم الوصفة قبل كده)
    public async Task<FeedbackResponse?> GetMyFeedbackAsync(int userId, int recipeId, CancellationToken cancellationToken = default)
    {
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.UserId == userId, tracked: false, cancellationToken: cancellationToken);
        if (patient == null) return null;

        var feedback = await unitOfWork.FeedbackRepository.GetAsync(
            filter: f => f.RecipeId == recipeId && f.PatientId == patient.PatientId,
            tracked: false,
            includeProperties: "Patient.User",
            cancellationToken: cancellationToken);

        if (feedback == null) return null;

        return new FeedbackResponse
        {
            FeedbackId = feedback.FeedbackId,
            RecipeId = feedback.RecipeId,
            RatingValue = feedback.RatingValue,
            Comment = feedback.Comment,
            RatingDate = feedback.RatingDate,
            PatientName = feedback.Patient?.User?.FullName ?? "Unknown Patient"
        };
    }

    // 4️⃣ حذف تقييم المريض (وتعديل الحسبة)
    public async Task<bool> DeleteMyFeedbackAsync(int userId, int recipeId, CancellationToken cancellationToken = default)
    {
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.UserId == userId, tracked: false, cancellationToken: cancellationToken);
        if (patient == null) return false;

        var feedback = await unitOfWork.FeedbackRepository.GetAsync(
            filter: f => f.RecipeId == recipeId && f.PatientId == patient.PatientId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (feedback == null) return false;

        var recipe = await unitOfWork.RecipeRepository.GetAsync(r => r.RecipeId == recipeId, tracked: true, cancellationToken: cancellationToken);
        if (recipe != null)
        {
            // 🧮 حسبة الحذف: لو هو التقييم الوحيد، بنصفر الوصفة
            if (recipe.TotalRatings == 1)
            {
                recipe.AverageRating = 0;
                recipe.TotalRatings = 0;
            }
            else
            {
                // 👈 حسبة المتوسط الجديد بعد الحذف (مع التقريب لرقمين عشريين)
                var calculatedAverage = ((recipe.AverageRating * recipe.TotalRatings) - feedback.RatingValue) / (recipe.TotalRatings - 1);
                recipe.AverageRating = (float)Math.Round(calculatedAverage, 1);
                recipe.TotalRatings -= 1;
            }
        }

        unitOfWork.FeedbackRepository.Remove(feedback);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}