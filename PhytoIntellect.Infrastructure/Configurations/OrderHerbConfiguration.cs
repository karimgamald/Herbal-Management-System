using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class OrderHerbConfiguration : IEntityTypeConfiguration<OrderHerb>
{
    public void Configure(EntityTypeBuilder<OrderHerb> builder)
    {
        builder.HasKey(oh => oh.OrderHerbId);

        builder.Property(oh => oh.UnitPrice).HasColumnType("decimal(10,2)");
        builder.Property(oh => oh.SubTotal).HasColumnType("decimal(10,2)");

        builder.HasOne(oh => oh.SubOrder)
               .WithMany(s => s.OrderHerbs)
               .HasForeignKey(oh => oh.SubOrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(oh => oh.Herb)
               .WithMany()
               .HasForeignKey(oh => oh.HerbId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}