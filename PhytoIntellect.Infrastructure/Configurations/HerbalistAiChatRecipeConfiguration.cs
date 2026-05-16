using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class HerbalistAiChatRecipeConfiguration : IEntityTypeConfiguration<HerbalistAiChatRecipe>
{
    public void Configure(EntityTypeBuilder<HerbalistAiChatRecipe> builder)
    {
        builder.HasKey(x => new { x.HerbalistId, x.AiChatRecipeId });

        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.Property(x => x.IsActive).HasDefaultValue(true);

        builder.HasOne(x => x.Herbalist)
               .WithMany() 
               .HasForeignKey(x => x.HerbalistId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AiChatRecipe)
               .WithMany(x => x.HerbalistInventories)
               .HasForeignKey(x => x.AiChatRecipeId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}