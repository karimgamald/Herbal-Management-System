using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> UserRepository { get; }
    IPatientRepository PatientRepository { get; }
    IRepository<Herbalist> HerbalistRepository { get; }
    IRepository<RefreshToken> RefreshTokenRepository { get; }
    IRepository<MedicalHistory> MedicalHistoryRepository { get; }
    IRepository<Recipe> RecipeRepository { get; }
    IRepository<Disease> DiseaseRepository { get; }
    IRepository<Herb> HerbRepository { get; }
    IRepository<HerbalistHerb> HerbalistHerbRepository { get; }
    IRepository<RecipeHerb> RecipeHerbRepository { get; }
    IRepository<Feedback> FeedbackRepository { get; }
    IRepository<ReviewRecipe> ReviewRecipeRepository { get; }
    IRepository<Order> OrderRepository { get; }
    IRepository<SubOrder> SubOrderRepository { get; }
    IRepository<AiRecipe> AiRecipeRepository { get; }
    IRepository<HerbalistAiRecipe> HerbalistAiRecipeRepository { get; }
    IRepository<OrderAiRecipe> OrderAiRecipeRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}