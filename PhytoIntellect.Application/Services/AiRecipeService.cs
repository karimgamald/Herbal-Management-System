using AutoMapper;
using PhytoIntellect.Application.Contracts.AiRecipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Services;

public class AiRecipeService(
    IUnitOfWork unitOfWork, 
    IAiPredictionService aiPredictionService,IMapper mapper) 
    : IAiRecipeService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAiPredictionService _aiPredictionService = aiPredictionService;
    private readonly IMapper _mapper = mapper;
    // Get all AI recipes in the system
    public async Task<IEnumerable<AiRecipeResponse>> GetAllPublicAsync(CancellationToken cancellationToken = default)
    {
        var recipes = await _unitOfWork.AiRecipeRepository.GetAllAsync(
            tracked: false,
            cancellationToken: cancellationToken
        );

        // ترتيب بالأحدث
        recipes = await _unitOfWork.AiRecipeRepository.GetAllAsync(tracked: false,cancellationToken:cancellationToken);

        var orderedRecipes = recipes
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

        return _mapper.Map<IEnumerable<AiRecipeResponse>>(recipes);
    }
    public async Task<AiRecipeResponse> GetPublicByIdAsync(int recipeId,
    CancellationToken cancellationToken = default)
    {
        var recipe = await _unitOfWork.AiRecipeRepository.GetAsync(
            filter: r => r.Id == recipeId,
            tracked: false,
            cancellationToken: cancellationToken
        );

        if (recipe == null)
            throw new KeyNotFoundException("AI Recipe not found.");

        return _mapper.Map<AiRecipeResponse>(recipe);
    }

    // Get all recipes added by the patient 
    public async Task<IEnumerable<AiRecipeResponse>> GetAllAsync(int userId,CancellationToken cancellationToken = default)
    {
        int patientId = await _unitOfWork.PatientRepository
            .GetIdByUserIdAsync(userId.ToString());

        if (patientId == 0)
            throw new UnauthorizedAccessException("Patient profile not found.");

        var recipes = await _unitOfWork.AiRecipeRepository.GetAllAsync(
            filter: r => r.PatientId == patientId,
            tracked: false,
            cancellationToken: cancellationToken
        );

        return _mapper.Map<IEnumerable<AiRecipeResponse>>(recipes);
    }
    // Get the 
    public async Task<AiRecipeResponse> GetByIdAsync(int userId,int recipeId,CancellationToken cancellationToken = default)
    {
        int patientId = await _unitOfWork.PatientRepository
            .GetIdByUserIdAsync(userId.ToString());

        if (patientId == 0)
            throw new UnauthorizedAccessException("Patient profile not found.");

        var recipe = await _unitOfWork.AiRecipeRepository.GetAsync(
            filter: r => r.Id == recipeId && r.PatientId == patientId,
            tracked: false,
            cancellationToken: cancellationToken
        );

        if (recipe == null)
            throw new UnauthorizedAccessException("Recipe not found or access denied.");

        return _mapper.Map<AiRecipeResponse>(recipe);
    }
    public async Task<AiRecipeResponse> GenerateRecipeAsync(int userId, CreateAiRecipeRequest request)
    {
        int patientId = await _unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());

        if (patientId == 0)
            throw new UnauthorizedAccessException("Patient profile not found for this user.");

        var patient = await _unitOfWork.PatientRepository.GetPatientWithHistoryAsync(patientId);

        if (patient == null)
            throw new Exception("Patient not found.");

        if (!patient.BirthDate.HasValue) 
            throw new InvalidOperationException("PROFILE_INCOMPLETE_DOB");

        if (string.IsNullOrWhiteSpace(patient.Gender.ToString()))
            throw new InvalidOperationException("PROFILE_INCOMPLETE_GENDER");

        if (patient.MedicalHistory == null)
            throw new InvalidOperationException("MEDICAL_HISTORY_MISSING");



        var bday = patient.BirthDate.Value;
        var today = DateTime.Today;

        int calculatedAge = today.Year - bday.Year;

        if (bday.Month > today.Month || (bday.Month == today.Month && bday.Day > today.Day))
        {
            calculatedAge--; 
        }

        var aiInput = new AiEngineInput
        {
            Age = calculatedAge, 
            Gender = patient.Gender.ToString(),
            HasDiabetes = patient.MedicalHistory.Diabetes,
            HasHypertension = patient.MedicalHistory.Hypertension,
            HasAllergies = patient.MedicalHistory.Asthma,
            IsPregnant = patient.MedicalHistory.Pregnancy,
            IsSmoker = patient.MedicalHistory.Smoker,
            CurrentVitals = request 
        };

        var predictionResult = await _aiPredictionService.GetPredictionAsync(aiInput);

        var heightInMeters = request.HeightCm / 100.0;
        var bmi = request.WeightKg / (heightInMeters * heightInMeters);

        var recipeRecord = new AiRecipe
        {
            PatientId = patientId,
            Age = calculatedAge,
            Gender = aiInput.Gender,
            WeightKg = request.WeightKg,
            HeightCm = request.HeightCm,
            Bmi = Math.Round(bmi, 1),
            SeverityScore = request.SeverityScore,
            SystolicBp = request.SystolicBp,
            DiastolicBp = request.DiastolicBp,
            TemperatureCelsius = request.TemperatureCelsius,
            HeartRateBpm = request.HeartRateBpm,
            SymptomDurationDays = request.SymptomDurationDays,

            HasDiabetes = aiInput.HasDiabetes,
            HasHypertension = aiInput.HasHypertension,
            HasAllergies = aiInput.HasAllergies,
            IsPregnant = aiInput.IsPregnant,
            IsSmoker = aiInput.IsSmoker,

            Symptoms = request.SelectedSymptoms,

            RecommendedRecipeName = predictionResult.RecommendedRecipeName,
            Condition = predictionResult.Condition,
            ConfidenceScore = predictionResult.ConfidenceScore,
            PreparationInstructions = predictionResult.PreparationInstructions,
            CautionWarning = predictionResult.CautionWarning
        };

        await _unitOfWork.AiRecipeRepository.CreateAsync(recipeRecord);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AiRecipeResponse>(recipeRecord);
    }

    //public async Task DeleteAsync(int recipeId, CancellationToken cancellationToken = default)
    //{
    //    var recipe = await _unitOfWork.AiRecipeRepository.GetByIdAsync(recipeId);

    //    if (recipe == null)
    //        throw new KeyNotFoundException("AI Recipe not found.");

    //    await _unitOfWork.AiRecipeRepository.DeleteAsync(recipe);

    //    await _unitOfWork.SaveChangesAsync();
    //}
}