using PhytoIntellect.Application.Contracts.AiRecipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Services;

public class AiRecipeService : IAiRecipeService
{
    private readonly IUnitOfWork _unitOfWork; // غيرناها من DbContext لـ UnitOfWork
    private readonly IAiPredictionService _aiPredictionService;

    public AiRecipeService(IUnitOfWork unitOfWork, IAiPredictionService aiPredictionService)
    {
        _unitOfWork = unitOfWork;
        _aiPredictionService = aiPredictionService;
    }

    public async Task<AiRecipeResponse> GenerateRecipeAsync(CreateAiRecipeRequest request)
    {
        // 1. نكلم الـ AI مباشرة (الـ Wrapper اللي في الـ Infra هو اللي هيترجم اللستة للفلاسك)
        var predictionResult = await _aiPredictionService.GetPredictionAsync(request);

        // 2. حساب الـ BMI عشان نسيفه في الداتابيز
        var heightInMeters = request.HeightCm / 100.0;
        var bmi = request.WeightKg / (heightInMeters * heightInMeters);

        // 3. نسيف النتيجة في الداتابيز بتاعتنا (الـ Entity)
        var recipeRecord = new AiRecipe
        {
            PatientId = request.PatientId,
            Age = request.Age,
            Gender = request.Gender,
            WeightKg = request.WeightKg,
            HeightCm = request.HeightCm,
            Bmi = Math.Round(bmi, 1),
            SeverityScore = request.SeverityScore,
            SystolicBp = request.SystolicBp,
            DiastolicBp = request.DiastolicBp,
            TemperatureCelsius = request.TemperatureCelsius,
            HeartRateBpm = request.HeartRateBpm,
            SymptomDurationDays = request.SymptomDurationDays,
            HasDiabetes = request.HasDiabetes,
            HasHypertension = request.HasHypertension,
            HasAllergies = request.HasAllergies,
            IsPregnant = request.IsPregnant,
            IsSmoker = request.IsSmoker,
            Symptoms = request.SelectedSymptoms, // هتتسيف كـ JSON أوتوماتيك

            // مخرجات الـ AI اللي راجعة من الـ Wrapper النظيف
            RecommendedRecipeName = predictionResult.RecommendedRecipeName,
            Condition = predictionResult.Condition,
            ConfidenceScore = predictionResult.ConfidenceScore,
            PreparationInstructions = predictionResult.PreparationInstructions,
            CautionWarning = predictionResult.CautionWarning
        };

        // 4. الحفظ باستخدام الـ UnitOfWork
        await _unitOfWork.AiRecipeRepository.CreateAsync(recipeRecord);
        await _unitOfWork.SaveChangesAsync(); // أو SaveChangesAsync() حسب ما إنت مسميها

        // 5. نرجع الرد للـ Controller ومعاه الـ ID الجديد
        return new AiRecipeResponse
        {
            RecipeId = recipeRecord.Id,
            RecommendedRecipeName = recipeRecord.RecommendedRecipeName,
            Condition = recipeRecord.Condition,
            ConfidenceScore = recipeRecord.ConfidenceScore,
            PreparationInstructions = recipeRecord.PreparationInstructions,
            CautionWarning = recipeRecord.CautionWarning
        };
    }
}