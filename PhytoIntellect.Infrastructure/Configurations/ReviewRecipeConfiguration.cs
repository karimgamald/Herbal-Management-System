using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class ReviewRecipeConfiguration : IEntityTypeConfiguration<ReviewRecipe>
{
    public void Configure(EntityTypeBuilder<ReviewRecipe> builder)
    {
        builder.HasKey(r => r.ReviewRecipeId);
        builder.Property(r => r.Comment).HasMaxLength(1000);

        // 🚨 منع التكرار: العطار ميقيمش نفس الوصفة مرتين
        builder.HasIndex(r => new { r.RecipeId, r.HerbalistId }).IsUnique();

        // 🚨 التقييم من 1 لـ 5 بس
        builder.ToTable("ReviewRecipes", t => t.HasCheckConstraint("CK_ReviewRecipe_RatingValue", "[RatingValue] >= 1 AND [RatingValue] <= 5"));

        builder.HasOne(r => r.Recipe).WithMany(rec => rec.Reviews)
               .HasForeignKey(r => r.RecipeId).OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Herbalist).WithMany()
               .HasForeignKey(r => r.HerbalistId).OnDelete(DeleteBehavior.Restrict);

        // الكود القديم كان .WithMany() فاضي
        // الكود الجديد:
        builder.HasOne(r => r.Herbalist)
               .WithMany(h => h.ReviewRecipes) // 👈 عرفناه إن دي الليستة بتاعته
               .HasForeignKey(r => r.HerbalistId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}