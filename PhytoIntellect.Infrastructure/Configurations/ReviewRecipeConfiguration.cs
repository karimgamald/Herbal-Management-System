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

        builder.HasIndex(r => new { r.AiRecipeId, r.HerbalistId }).IsUnique();

        builder.ToTable("ReviewRecipes", t => t.HasCheckConstraint("CK_ReviewRecipe_RatingValue", "[RatingValue] >= 1 AND [RatingValue] <= 5"));

        builder.HasOne(r => r.AiRecipe)
               .WithMany(a => a.HerbalistReviews)
               .HasForeignKey(r => r.AiRecipeId)
               .OnDelete(DeleteBehavior.Cascade); 

        builder.HasOne(r => r.Herbalist)
               .WithMany(h => h.ReviewRecipes)
               .HasForeignKey(r => r.HerbalistId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}