using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using PhytoIntellect.Infrastructure.Presistence;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Infrastructure.UOW;

public class UnitOfWork(
    ApplicationDbContext context,
    IRepository<User> userRepository,
    //IRepository<Patient> patientRepository,
    IPatientRepository patientRepository,
    IRepository<Herbalist> herbalistRepository,
    IRepository<RefreshToken> refreshTokenRepository,
    IRepository<MedicalHistory> medicalHistoryRepository,
    IRepository<Recipe> recipeRepository,
    IRepository<RecipeHerb> recipeHerbRepository,
    IRepository<Herb> herbRepository,
    IRepository<HerbalistHerb> herbalistHerbRepository,
    IRepository<Disease> diseaseRepository,
    IRepository<Feedback> feedbackRepository,
    IRepository<ReviewRecipe> reviewRecipeRepository,
    IRepository<Order> orderRepository,
    IRepository<SubOrder> subOrderRepository,
    IRepository<AiRecipe> aiRecipeRepository
    ) : IUnitOfWork
{
    public IRepository<User> UserRepository { get; } = userRepository;
    //public IRepository<Patient> PatientRepository { get; } = patientRepository;
    public IPatientRepository PatientRepository { get; } = patientRepository;
    public IRepository<Herbalist> HerbalistRepository { get; } = herbalistRepository;
    public IRepository<RefreshToken> RefreshTokenRepository { get; } = refreshTokenRepository;
    public IRepository<MedicalHistory> MedicalHistoryRepository { get; } = medicalHistoryRepository;
    public IRepository<Recipe> RecipeRepository { get; } = recipeRepository;
    public IRepository<Disease> DiseaseRepository { get; } = diseaseRepository;
    public IRepository<Herb> HerbRepository { get; } = herbRepository;
    public IRepository<HerbalistHerb> HerbalistHerbRepository { get; } = herbalistHerbRepository; 
    public IRepository<RecipeHerb> RecipeHerbRepository { get; } = recipeHerbRepository;
    public IRepository<Feedback> FeedbackRepository { get; } = feedbackRepository;
    public IRepository<ReviewRecipe> ReviewRecipeRepository { get; } = reviewRecipeRepository;
    public IRepository<Order> OrderRepository { get; } = orderRepository;
    public IRepository<SubOrder> SubOrderRepository { get; } = subOrderRepository;
    public IRepository<AiRecipe> AiRecipeRepository { get; } = aiRecipeRepository;


    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        context.Dispose();
    }
}