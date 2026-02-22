using Microsoft.EntityFrameworkCore;
using PhytoIntellect.Core.Entities;
using System.Reflection;


namespace PhytoIntellect.Infrastructure.Presistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Herbalist> Herbalists { get; set; }
    public DbSet<MedicalHistory> MedicalHistories { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);

    }
}
