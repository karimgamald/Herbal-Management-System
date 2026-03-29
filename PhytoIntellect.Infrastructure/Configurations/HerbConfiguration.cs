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
        builder.Property(h => h.HerbName).HasMaxLength(100).IsRequired();
        builder.Property(h => h.ScientificName).HasMaxLength(150);
        builder.Property(h => h.Dosage).HasMaxLength(100);

        builder.Property(h => h.IsApproved).HasDefaultValue(false);

        builder.HasOne(h => h.AddedByHerbalist)
               .WithMany() 
               .HasForeignKey(h => h.AddedByHerbalistId)
               .OnDelete(DeleteBehavior.SetNull);

    }
}