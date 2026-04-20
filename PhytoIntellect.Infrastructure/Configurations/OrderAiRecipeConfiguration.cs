using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class OrderAiRecipeConfiguration : IEntityTypeConfiguration<OrderAiRecipe>
{
    public void Configure(EntityTypeBuilder<OrderAiRecipe> builder)
    {
        builder.HasKey(x => x.OrderAiRecipeId);

        builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(x => x.SubTotal).HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.SubOrder)
               .WithMany(s => s.OrderAiRecipes)
               .HasForeignKey(x => x.SubOrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AiRecipe)
               .WithMany()
               .HasForeignKey(x => x.AiRecipeId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}