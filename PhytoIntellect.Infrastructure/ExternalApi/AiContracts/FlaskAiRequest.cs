using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PhytoIntellect.Infrastructure.ExternalApi.AiContracts;

public class FlaskAiRequest
{
    // --- البيانات الأساسية ---
    [JsonPropertyName("age")] public int Age { get; set; }
    [JsonPropertyName("gender")] public string Gender { get; set; }
    [JsonPropertyName("weight_kg")] public double WeightKg { get; set; }
    [JsonPropertyName("height_cm")] public double HeightCm { get; set; }
    [JsonPropertyName("bmi")] public double Bmi { get; set; }
    [JsonPropertyName("severity_score")] public int SeverityScore { get; set; }

    // --- العلامات الحيوية ---
    [JsonPropertyName("blood_pressure_systolic")] public int BloodPressureSystolic { get; set; }
    [JsonPropertyName("blood_pressure_diastolic")] public int BloodPressureDiastolic { get; set; }
    [JsonPropertyName("temperature_celsius")] public double TemperatureCelsius { get; set; }
    [JsonPropertyName("heart_rate_bpm")] public int HeartRateBpm { get; set; }
    [JsonPropertyName("symptom_duration_days")] public int SymptomDurationDays { get; set; }

    // --- التاريخ المرضي ---
    [JsonPropertyName("has_diabetes")] public int HasDiabetes { get; set; }
    [JsonPropertyName("has_hypertension")] public int HasHypertension { get; set; }
    [JsonPropertyName("has_allergy")] public int HasAllergy { get; set; }
    [JsonPropertyName("is_pregnant")] public int IsPregnant { get; set; }
    [JsonPropertyName("is_smoker")] public int IsSmoker { get; set; }

    // --- الـ 33 عرض (كلهم int عشان الفلاسك مستني 0 أو 1) ---
    [JsonPropertyName("itchy_eyes")] public int ItchyEyes { get; set; }
    [JsonPropertyName("visual_aura")] public int VisualAura { get; set; }
    [JsonPropertyName("runny_nose")] public int RunnyNose { get; set; }
    [JsonPropertyName("headache")] public int Headache { get; set; }
    [JsonPropertyName("watery_eyes")] public int WateryEyes { get; set; }
    [JsonPropertyName("sneezing")] public int Sneezing { get; set; }
    [JsonPropertyName("chest_pain")] public int ChestPain { get; set; }
    [JsonPropertyName("painful_urination")] public int PainfulUrination { get; set; }
    [JsonPropertyName("pelvic_pain")] public int PelvicPain { get; set; }
    [JsonPropertyName("severe_headache")] public int SevereHeadache { get; set; }
    [JsonPropertyName("light_sensitivity")] public int LightSensitivity { get; set; }
    [JsonPropertyName("cloud_urine")] public int CloudyUrine { get; set; }
    [JsonPropertyName("abdominal_pain")] public int AbdominalPain { get; set; }
    [JsonPropertyName("vomiting")] public int Vomiting { get; set; }
    [JsonPropertyName("fever")] public int Fever { get; set; }
    [JsonPropertyName("dizziness")] public int Dizziness { get; set; }
    [JsonPropertyName("cold_hands")] public int ColdHands { get; set; }
    [JsonPropertyName("cough")] public int Cough { get; set; }
    [JsonPropertyName("slow_healing")] public int SlowHealing { get; set; }
    [JsonPropertyName("cold_intolerance")] public int ColdIntolerance { get; set; }
    [JsonPropertyName("blurred_vision")] public int BlurredVision { get; set; }
    [JsonPropertyName("diarrhea")] public int Diarrhea { get; set; }
    [JsonPropertyName("weight_gain")] public int WeightGain { get; set; }
    [JsonPropertyName("nausea")] public int Nausea { get; set; }
    [JsonPropertyName("excessive_thirst")] public int ExcessiveThirst { get; set; }
    [JsonPropertyName("nasal_congestion")] public int NasalCongestion { get; set; }
    [JsonPropertyName("frequent_urination")] public int FrequentUrination { get; set; }
    [JsonPropertyName("dry_skin")] public int DrySkin { get; set; }
    [JsonPropertyName("pale_skin")] public int PaleSkin { get; set; }
    [JsonPropertyName("fatigue")] public int Fatigue { get; set; }
    [JsonPropertyName("constipation")] public int Constipation { get; set; }
    [JsonPropertyName("sore_throat")] public int SoreThroat { get; set; }
    [JsonPropertyName("shortness_of_breath")] public int ShortnessOfBreath { get; set; }
}