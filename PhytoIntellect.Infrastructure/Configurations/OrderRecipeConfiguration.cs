using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class OrderRecipeConfiguration : IEntityTypeConfiguration<OrderRecipe>
{
    public void Configure(EntityTypeBuilder<OrderRecipe> builder)
    {
        builder.HasKey(or => or.OrderRecipeId);

        builder.Property(or => or.UnitPrice).HasColumnType("decimal(10,2)");
        builder.Property(or => or.SubTotal).HasColumnType("decimal(10,2)");

        builder.HasOne(or => or.SubOrder)
               .WithMany(s => s.OrderRecipes)
               .HasForeignKey(or => or.SubOrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(or => or.Recipe)
               .WithMany()
               .HasForeignKey(or => or.RecipeId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}