using System.Threading;
using System.Threading.Tasks;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Infrastructure.Presistence;

namespace PhytoIntellect.Infrastructure.UOW;

// شوف الحقن كله بقى فوق إزاي
public class UnitOfWork(
    ApplicationDbContext context,
    IRepository<User> userRepository,
    IRepository<Patient> patientRepository,
    IRepository<Herbalist> herbalistRepository,
    IRepository<RefreshToken> refreshTokenRepository,
    IRepository<MedicalHistory> medicalHistoryRepository,
    IRepository<Recipe> recipeRepository,
    IRepository<RecipeHerb> recipeHerbRepository) : IUnitOfWork
{
    // ربطنا الخصائص بالـ Parameters اللي جيالنا من فوق
    public IRepository<User> UserRepository { get; } = userRepository;
    public IRepository<Patient> PatientRepository { get; } = patientRepository;
    public IRepository<Herbalist> HerbalistRepository { get; } = herbalistRepository;
    public IRepository<RefreshToken> RefreshTokenRepository { get; } = refreshTokenRepository;
    public IRepository<MedicalHistory> MedicalHistoryRepository { get; } = medicalHistoryRepository;
    public IRepository<Recipe> RecipeRepository { get; } = recipeRepository;
    public IRepository<RecipeHerb> RecipeHerbRepository { get; } = recipeHerbRepository;
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // هنا الداتابيز بتضرب لو اليوزر لغى الريكويست
        return await context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        context.Dispose();
    }
}