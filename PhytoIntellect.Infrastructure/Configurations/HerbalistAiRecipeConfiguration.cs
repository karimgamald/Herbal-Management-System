using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class HerbalistAiRecipeConfiguration : IEntityTypeConfiguration<HerbalistAiRecipe>
{
    public void Configure(EntityTypeBuilder<HerbalistAiRecipe> builder)
    {
        builder.HasKey(x => new { x.HerbalistId, x.AiRecipeId });

        builder.Property(x => x.Price)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.HasOne(x => x.Herbalist)
               .WithMany(h => h.HerbalistAiRecipes) 
               .HasForeignKey(x => x.HerbalistId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AiRecipe)
               .WithMany(a => a.HerbalistInventories)
               .HasForeignKey(x => x.AiRecipeId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}