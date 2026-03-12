using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class HerbConfiguration : IEntityTypeConfiguration<Herb>
{
    public void Configure(EntityTypeBuilder<Herb> builder)
    {
        // تحديد أطوال النصوص عشان منستهلكش مساحة عالفاضي
        builder.Property(h => h.HerbName).HasMaxLength(100).IsRequired();
        builder.Property(h => h.ScientificName).HasMaxLength(150);
        builder.Property(h => h.Dosage).HasMaxLength(100);

        // العشبة الجديدة بتكون قيد المراجعة افتراضياً
        builder.Property(h => h.IsApproved).HasDefaultValue(false);

        // علاقة العطار اللي ضاف العشبة (ممكن تكون Null لو السيستم هو اللي ضايفها)
        builder.HasOne(h => h.AddedByHerbalist)
               .WithMany() // مفيش داعي نعمل Collection في كلاس العطار للأعشاب اللي اقترحها
               .HasForeignKey(h => h.AddedByHerbalistId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}