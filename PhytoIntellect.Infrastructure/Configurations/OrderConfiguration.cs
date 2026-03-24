using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.OrderId);

        // ضبط الفلوس
        builder.Property(o => o.ItemsTotal).HasColumnType("decimal(10,2)");
        builder.Property(o => o.DeliveryFee).HasColumnType("decimal(10,2)");
        builder.Property(o => o.TotalPrice).HasColumnType("decimal(10,2)");

        // ضبط النصوص
        builder.Property(o => o.PaymentMethod).HasMaxLength(50);
        builder.Property(o => o.PaymentStatus).HasMaxLength(50);
        builder.Property(o => o.ExternalPaymentID).HasMaxLength(100);
        builder.Property(o => o.ExternalDeliveryID).HasMaxLength(100);
        builder.Property(o => o.OrderStatus).HasMaxLength(50);

        // العنوان نخليه كبير عشان ممكن يكون تفصيلي
        builder.Property(o => o.ShippingAddress).IsRequired();

        // العلاقات
        builder.HasOne(o => o.Patient)
               .WithMany() // لو عامل Collection في الـ Patient حطها هنا
               .HasForeignKey(o => o.PatientId)
               .OnDelete(DeleteBehavior.Restrict); // عشان لو مسحنا المريض الأوردر ميطيرش
    }
}