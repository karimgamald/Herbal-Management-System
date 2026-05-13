using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System.Text.Json;


namespace PhytoIntellect.Infrastructure.Configurations;

public class AiChatRecipeConfiguration : IEntityTypeConfiguration<AiChatRecipe>
{
    public void Configure(EntityTypeBuilder<AiChatRecipe> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Patient)
               .WithMany()
               .HasForeignKey(x => x.PatientId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.UserPrompt).HasMaxLength(300).IsRequired();
        builder.Property(x => x.RecommendedRecipeName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.MainHerb).HasMaxLength(50);
        builder.Property(x => x.ScientificName).HasMaxLength(100);
        builder.Property(x => x.Category).HasMaxLength(100);
        builder.Property(x => x.Contraindications).IsRequired(false);
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.Property(x => x.IsAvailable).HasDefaultValue(false);

        var stringListComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1 != null && c2 != null ? c1.SequenceEqual(c2) : c1 == c2,
            c => c != null ? c.Aggregate(0, (a, v) => HashCode.Combine(a, v != null ? v.GetHashCode() : 0)) : 0,
            c => c != null ? c.ToList() : new List<string>());

        builder.Property(x => x.OtherPossibilities)
               .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                   v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>()
               )
               .HasColumnType("nvarchar(max)")
               .Metadata.SetValueComparer(stringListComparer);

        //builder.HasMany(x => x.Feedbacks)
        //       .WithOne(f => f.AiChatRecipe)
        //       .HasForeignKey(f => f.AiChatRecipeId)
        //       .OnDelete(DeleteBehavior.Cascade);

        //builder.HasMany(x => x.HerbalistReviews)
        //       .WithOne(r => r.AiChatRecipe)
        //       .HasForeignKey(r => r.AiChatRecipeId)
        //       .OnDelete(DeleteBehavior.Cascade);
    }
}