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
    //IUserRepository userRepository,
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
    IRepository<AiRecipe> aiRecipeRepository,
    IRepository<AiChatRecipe> aiChatRecipeRepository,
    IRepository<HerbalistAiRecipe> herbalistAiRecipeRepository,
    IRepository<HerbalistAiChatRecipe> herbalistAiChatRecipeRepository,
    IRepository<OrderAiRecipe> orderAiRecipeRepository,
    IRepository<UserFavorite> userFavoriteRepository
    ) : IUnitOfWork
{ 
    public IRepository<User> UserRepository { get; } = userRepository;
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
    public IRepository<AiChatRecipe> AiChatRecipeRepository { get; } = aiChatRecipeRepository;
    public IRepository<HerbalistAiRecipe> HerbalistAiRecipeRepository { get; } = herbalistAiRecipeRepository;
    public IRepository<HerbalistAiChatRecipe> HerbalistAiChatRecipeRepository { get; } = herbalistAiChatRecipeRepository;
    public IRepository<OrderAiRecipe> OrderAiRecipeRepository { get; } = orderAiRecipeRepository;
    public IRepository<UserFavorite> UserFavoriteRepository { get; } = userFavoriteRepository;


    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        context.Dispose();
    }
}