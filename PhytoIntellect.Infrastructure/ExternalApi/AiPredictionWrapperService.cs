using PhytoIntellect.Application.Contracts.AiRecipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Infrastructure.ExternalApi.AiContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.ExternalApi;

public class AiPredictionWrapperService(IAiFlaskClient flaskClient) : IAiPredictionService
{
    private readonly IAiFlaskClient _flaskClient = flaskClient;


    public async Task<AiPredictionResult> GetPredictionAsync(CreateAiRecipeRequest request)
    {
        // 1. تحويل الطلب النظيف (بتاع الفلاتر) لـ طلب الفلاسك المعقد (بتاع البايثون)
        var flaskRequest = MapToFlaskRequest(request);

        // 2. بننادي على الفلاسك (بياخد FlaskAiRequest ويرجع FlaskAiResponse)
        var flaskResponse = await _flaskClient.GetPredictionAsync(flaskRequest);

        // 3. بنترجم الرد اللي جاي من الفلاسك لرد نظيف عشان الـ Application يفهمه
        return new AiPredictionResult
        {
            RecommendedRecipeName = flaskResponse.Recipe.Name,
            Condition = flaskResponse.Recipe.Condition,
            ConfidenceScore = flaskResponse.Confidence,
            PreparationInstructions = flaskResponse.Recipe.Instructions,
            CautionWarning = flaskResponse.Recipe.Caution
        };
    }

    // ==========================================
    // ⚙️ دالة الترجمة (Mapping) اللي بتوزع الأعراض
    // ==========================================
    private FlaskAiRequest MapToFlaskRequest(CreateAiRecipeRequest req)
    {
        // حساب الـ BMI
        double heightInMeters = req.HeightCm / 100.0;
        double bmi = req.WeightKg / (heightInMeters * heightInMeters);

        var flaskReq = new FlaskAiRequest
        {
            Age = req.Age,
            Gender = req.Gender,
            WeightKg = req.WeightKg,
            HeightCm = req.HeightCm,
            Bmi = Math.Round(bmi, 1),
            SeverityScore = req.SeverityScore,
            BloodPressureSystolic = req.SystolicBp,
            BloodPressureDiastolic = req.DiastolicBp,
            TemperatureCelsius = req.TemperatureCelsius,
            HeartRateBpm = req.HeartRateBpm,
            SymptomDurationDays = req.SymptomDurationDays,

            // تحويل الـ bool لـ 0 و 1 عشان الفلاسك
            HasDiabetes = req.HasDiabetes ? 1 : 0,
            HasHypertension = req.HasHypertension ? 1 : 0,
            HasAllergy = req.HasAllergies ? 1 : 0,
            IsPregnant = req.IsPregnant ? 1 : 0,
            IsSmoker = req.IsSmoker ? 1 : 0
        };

        // --- السحر هنا: توزيع الأعراض أوتوماتيك ---
        // بنشيل المسافات من الأعراض اللي جاية من الموبايل ونخليها حروف صغيرة (مثلاً Itchy Eyes -> itchyeyes)
        var activeSymptoms = req.SelectedSymptoms
            .Select(s => s.Replace(" ", "").ToLower())
            .ToList();

        // بنلف على كل خصائص الـ FlaskAiRequest (اللي هما الـ 33 عرض)
        var properties = typeof(FlaskAiRequest).GetProperties();

        foreach (var prop in properties)
        {
            // لو اسم الخاصية (مثلاً ItchyEyes -> itchyeyes) موجودة في لستة المريض
            if (activeSymptoms.Contains(prop.Name.ToLower()) && prop.PropertyType == typeof(int))
            {
                // بنديها قيمة 1
                prop.SetValue(flaskReq, 1);
            }
        }

        return flaskReq;
    }
}
