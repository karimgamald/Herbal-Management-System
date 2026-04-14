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

    public async Task<AiPredictionResult> GetPredictionAsync(AiEngineInput request) 
    {
        var flaskRequest = MapToFlaskRequest(request);
        var flaskResponse = await _flaskClient.GetPredictionAsync(flaskRequest);

        return new AiPredictionResult
        {
            RecommendedRecipeName = flaskResponse.Recipe.Name,
            Condition = flaskResponse.Recipe.Condition,
            ConfidenceScore = flaskResponse.Confidence,
            PreparationInstructions = flaskResponse.Recipe.Instructions,
            CautionWarning = flaskResponse.Recipe.Caution
        };
    }

    private FlaskAiRequest MapToFlaskRequest(AiEngineInput req)
    {
        double heightInMeters = req.CurrentVitals.HeightCm / 100.0;
        double bmi = req.CurrentVitals.WeightKg / (heightInMeters * heightInMeters);

        var flaskReq = new FlaskAiRequest
        {
            Age = req.Age,
            Gender = req.Gender,
            HasDiabetes = req.HasDiabetes ? 1 : 0,
            HasHypertension = req.HasHypertension ? 1 : 0,
            HasAllergy = req.HasAllergies ? 1 : 0,
            IsPregnant = req.IsPregnant ? 1 : 0,
            IsSmoker = req.IsSmoker ? 1 : 0,

            WeightKg = req.CurrentVitals.WeightKg,
            HeightCm = req.CurrentVitals.HeightCm,
            Bmi = Math.Round(bmi, 1),
            SeverityScore = req.CurrentVitals.SeverityScore,
            BloodPressureSystolic = req.CurrentVitals.SystolicBp,
            BloodPressureDiastolic = req.CurrentVitals.DiastolicBp,
            TemperatureCelsius = req.CurrentVitals.TemperatureCelsius,
            HeartRateBpm = req.CurrentVitals.HeartRateBpm,
            SymptomDurationDays = req.CurrentVitals.SymptomDurationDays,
        };

        var activeSymptoms = req.CurrentVitals.SelectedSymptoms
            .Select(s => s.Replace(" ", "").ToLower())
            .ToList();

        var properties = typeof(FlaskAiRequest).GetProperties();

        foreach (var prop in properties)
        {
            if (activeSymptoms.Contains(prop.Name.ToLower()) && prop.PropertyType == typeof(int))
            {
                prop.SetValue(flaskReq, 1);
            }
        }

        return flaskReq;
    }
}