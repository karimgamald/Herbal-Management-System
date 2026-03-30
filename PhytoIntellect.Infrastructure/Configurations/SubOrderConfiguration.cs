using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class SubOrderConfiguration : IEntityTypeConfiguration<SubOrder>
{
    public void Configure(EntityTypeBuilder<SubOrder> builder)
    {
        builder.HasKey(s => s.SubOrderId);

        builder.Property(s => s.SubTotal).HasColumnType("decimal(10,2)");
        builder.Property(s => s.Status).HasMaxLength(50);
        builder.Property(s => s.ExternalDeliveryID).HasMaxLength(100);

        builder.HasOne(s => s.Order)
               .WithMany(o => o.SubOrders)
               .HasForeignKey(s => s.OrderId)
               .OnDelete(DeleteBehavior.Cascade); // لو الأوردر الأساسي اتمسح، امسح الفرعي

        builder.HasOne(s => s.Herbalist)
               .WithMany()
               .HasForeignKey(s => s.HerbalistId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}