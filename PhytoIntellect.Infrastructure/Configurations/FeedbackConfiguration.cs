using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.HasKey(f => f.FeedbackId);
        builder.Property(f => f.Comment).HasMaxLength(1000);

        builder.ToTable("Feedbacks", t =>
        {
            t.HasCheckConstraint("CK_Feedback_RatingValue", "[RatingValue] >= 1 AND [RatingValue] <= 5");

            t.HasCheckConstraint("CK_Feedback_Target", "([RecipeId] IS NOT NULL AND [AiRecipeId] IS NULL) OR ([RecipeId] IS NULL AND [AiRecipeId] IS NOT NULL)");
        });

        builder.HasIndex(f => new { f.RecipeId, f.PatientId })
               .IsUnique()
               .HasFilter("[RecipeId] IS NOT NULL"); 

        builder.HasIndex(f => new { f.AiRecipeId, f.PatientId })
               .IsUnique()
               .HasFilter("[AiRecipeId] IS NOT NULL");
        
        builder.HasOne(f => f.Recipe)
               .WithMany(r => r.Feedbacks)
               .HasForeignKey(f => f.RecipeId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.AiRecipe)
               .WithMany(a => a.Feedbacks)
               .HasForeignKey(f => f.AiRecipeId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.Patient)
               .WithMany(p => p.Feedbacks)
               .HasForeignKey(f => f.PatientId)
               .OnDelete(DeleteBehavior.Restrict);
    }
} 