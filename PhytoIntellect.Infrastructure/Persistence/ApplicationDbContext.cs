using Microsoft.EntityFrameworkCore;
using PhytoIntellect.Core.Entities;
using System.Reflection;


namespace PhytoIntellect.Infrastructure.Presistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Herbalist> Herbalists { get; set; }
    public DbSet<MedicalHistory> MedicalHistories { get; set; }
    public DbSet<Herb> Herbs { get; set; }
    public DbSet<HerbalistHerb> HerbalistHerbs { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<RecipeHerb> RecipeHerbs { get; set; }
    public DbSet<Disease> Diseases { get; set; }
    public DbSet<RecipeDisease> RecipeDiseases { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<ReviewRecipe> ReviewRecipes { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<SubOrder> SubOrders { get; set; }
    public DbSet<OrderRecipe> OrderRecipes { get; set; }
    public DbSet<OrderHerb> OrderHerbs { get; set; }
    public DbSet<AiRecipe> AiRecipes { get; set; }
    public DbSet<HerbalistAiRecipe> HerbalistAiRecipes { get; set; }
    public DbSet<OrderAiRecipe> OrderAiRecipes { get; set; }
    public DbSet<UserFavorite> UserFavorites { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
