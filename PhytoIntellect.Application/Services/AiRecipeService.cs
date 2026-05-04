using AutoMapper;
using AutoMapper.QueryableExtensions;
using PhytoIntellect.Application.Contracts.AiRecipes;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
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

    public async Task<PaginatedList<AiRecipeResponse>> GetAllPublicAsync(
    RequestFilters filters,
    CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.AiRecipeRepository.GetQueryable(tracked: false);

        // لو عندك عمود اسمه IsPublic مثلاً بيحدد إن الوصفة عامة مش خاصة بمريض، تقدر تفلتر بيه هنا
        // query = query.Where(r => r.IsPublic == true); 

        // البحث (Searching)
        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(r => r.RecommendedRecipeName.ToLower().Contains(search));
        }

        // الترتيب (Sorting)
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
            query = filters.SortColumn.ToLower() switch
            {
                "recommendedRecipeName" => isDesc ? query.OrderByDescending(r => r.RecommendedRecipeName) : query.OrderBy(r => r.RecommendedRecipeName),
                "date" => isDesc ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt),
                _ => query.OrderByDescending(r => r.CreatedAt) // الديفولت
            };
        }
        else
        {
            query = query.OrderByDescending(r => r.CreatedAt); // ترتيب ديفولت بالأحدث
        }

        // المابينج السحري
        var projectedQuery = query.ProjectTo<AiRecipeResponse>(_mapper.ConfigurationProvider);

        // تطبيق الـ Pagination
        return await PaginatedList<AiRecipeResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
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
    public async Task<PaginatedList<AiRecipeResponse>> GetAllAsync(
    int userId,
    RequestFilters filters,
    CancellationToken cancellationToken = default)
    {
        // 1. نجيب رقم المريض
        int patientId = await _unitOfWork.PatientRepository.GetIdByUserIdAsync(userId.ToString());

        if (patientId == 0)
            throw new UnauthorizedAccessException("Patient profile not found.");

        // 2. نبدأ الكويري
        var query = _unitOfWork.AiRecipeRepository.GetQueryable(tracked: false);

        // الفلتر الأساسي (وصفات المريض ده بس)
        query = query.Where(r => r.PatientId == patientId);

        // 3. البحث
        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(r => r.RecommendedRecipeName.ToLower().Contains(search));
        }

        // 4. الترتيب
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
            query = filters.SortColumn.ToLower() switch
            {
                "recommendedRecipeName" => isDesc ? query.OrderByDescending(r => r.RecommendedRecipeName) : query.OrderBy(r => r.RecommendedRecipeName),
                "date" => isDesc ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(r => r.CreatedAt);
        }

        // 5. المابينج
        var projectedQuery = query.ProjectTo<AiRecipeResponse>(_mapper.ConfigurationProvider);

        // 6. الـ Pagination
        return await PaginatedList<AiRecipeResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
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