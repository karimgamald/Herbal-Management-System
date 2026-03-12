using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class HerbalistHerbConfiguration : IEntityTypeConfiguration<HerbalistHerb>
{
    public void Configure(EntityTypeBuilder<HerbalistHerb> builder)
    {
        // 1. تحديد البرايمري كي المزدوج (عشان ده جدول ربط)
        builder.HasKey(hh => new { hh.HerbalistId, hh.HerbId });

        // 2. تظبيط السعر عشان يقبل كسور (قرش/سنت)
        builder.Property(hh => hh.Price)
               .HasColumnType("decimal(10,2)")
               .IsRequired();


        // 3. القيمة الافتراضية
        builder.Property(hh => hh.IsActive)
               .HasDefaultValue(true);

        // 4. العلاقات (تأكيد عشان ميعملش Cascade Delete يطير الداتابيز كلها بالغلط)
        builder.HasOne(hh => hh.Herbalist)
               .WithMany(h => h.HerbalistHerbs)
               .HasForeignKey(hh => hh.HerbalistId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(hh => hh.Herb)
               .WithMany(h => h.HerbalistHerbs)
               .HasForeignKey(hh => hh.HerbId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}