using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class RecipeDiseaseConfiguration : IEntityTypeConfiguration<RecipeDisease>
{
    public void Configure(EntityTypeBuilder<RecipeDisease> builder)
    {
        builder.HasKey(rd => rd.RecipeDiseaseId);

        // تأكيد العلاقات (عشان لو الـ EF Core اتلخبط)
        builder.HasOne(rd => rd.Recipe)
               .WithMany(r => r.RecipeDiseases)
               .HasForeignKey(rd => rd.RecipeId)
               .OnDelete(DeleteBehavior.Cascade); // لو مسحنا الوصفة، يمسح الربط بتاعها

        builder.HasOne(rd => rd.Disease)
               .WithMany(d => d.RecipeDiseases)
               .HasForeignKey(rd => rd.DiseaseId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}