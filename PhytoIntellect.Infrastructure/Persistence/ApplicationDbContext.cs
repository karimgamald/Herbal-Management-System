using Microsoft.EntityFrameworkCore;
using PhytoIntellect.Core.Entities;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;


namespace PhytoIntellect.Infrastructure.Presistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Tables
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.UserName)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(u => u.PasswordHash)
                  .IsRequired();

            entity.Property(u => u.Role)
                  .IsRequired()
                  .HasMaxLength(50);
        });
    }
}