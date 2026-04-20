using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
               .WithMany()
               .HasForeignKey(x => x.PatientId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Gender).HasMaxLength(15).IsRequired();
        builder.Property(x => x.Condition).HasMaxLength(150);
        builder.Property(x => x.RecommendedRecipeName).HasMaxLength(250);
        builder.Property(x => x.CautionWarning).IsRequired(false);
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        var stringListComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1 != null && c2 != null ? c1.SequenceEqual(c2) : c1 == c2,
            c => c != null ? c.Aggregate(0, (a, v) => HashCode.Combine(a, v != null ? v.GetHashCode() : 0)) : 0,
            c => c != null ? c.ToList() : new List<string>());

        builder.Property(x => x.Symptoms)
               .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                   v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>() // تأمين الـ Null
               )
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(stringListComparer);

        builder.Property(x => x.PreparationInstructions)
               .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                   v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>() // تأمين الـ Null
               )
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(stringListComparer); 

        builder.HasMany(x => x.Feedbacks)
               .WithOne(f => f.AiRecipe)
               .HasForeignKey(f => f.AiRecipeId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.HerbalistReviews)
               .WithOne(r => r.AiRecipe)
               .HasForeignKey(r => r.AiRecipeId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}