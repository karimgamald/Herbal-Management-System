using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class OrderAiChatRecipeConfiguration : IEntityTypeConfiguration<OrderAiChatRecipe>
{
    public void Configure(EntityTypeBuilder<OrderAiChatRecipe> builder)
    {
        builder.HasKey(x => x.OrderAiChatRecipeId);

        builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(x => x.SubTotal).HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.SubOrder)
               .WithMany(x => x.OrderAiChatRecipes)
               .HasForeignKey(x => x.SubOrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AiChatRecipe)
               .WithMany()
               .HasForeignKey(x => x.AiChatRecipeId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
