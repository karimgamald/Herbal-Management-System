using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{

    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.Property(r => r.CreatedByAI).HasDefaultValue(true);
        builder.Property(r => r.IsActive).HasDefaultValue(true);
        builder.Property(r => r.CreatedDate).HasDefaultValueSql("GETUTCDATE()"); // بيسجل الوقت أوتوماتيك

        // العلاقة مع العطار (اختيارية)
        builder.HasOne(r => r.Herbalist)
               .WithMany(h => h.Recipes)
               .HasForeignKey(r => r.HerbalistId)
               .IsRequired(false) // 👈 مهمة جداً لوصفات الذكاء الاصطناعي
               .OnDelete(DeleteBehavior.SetNull);

     
    }
}