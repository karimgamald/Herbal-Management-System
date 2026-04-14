using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PhytoIntellect.Infrastructure.Configurations;

public class AiRecipeConfiguration : IEntityTypeConfiguration<AiRecipe>
{
    public void Configure(EntityTypeBuilder<AiRecipe> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Patient)
               .WithMany() // لو عندك List<AiRecipe> في كلاس المريض حطها هنا
               .HasForeignKey(x => x.PatientId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Gender)
               .HasMaxLength(15)
               .IsRequired(); // Male / Female

        builder.Property(x => x.Condition)
               .HasMaxLength(150); // اسم المرض زي Migraine

        builder.Property(x => x.RecommendedRecipeName)
               .HasMaxLength(250); // اسم الوصفة

        builder.Property(x => x.CautionWarning)
               .IsRequired(false);

        builder.Property(x => x.AllProbabilitiesJson)
               .IsRequired(false);

        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.Symptoms)
               .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!), // وهو رايح الداتابيز
                   v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) // وهو راجع للـ C#
               )
               .HasColumnType("nvarchar(max)"); 

        builder.Property(x => x.PreparationInstructions)
               .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                   v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!)
               )
               .HasColumnType("nvarchar(max)");
    }
}