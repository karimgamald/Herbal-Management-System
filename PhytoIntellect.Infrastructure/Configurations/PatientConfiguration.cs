
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System.Reflection.Emit;

namespace PhytoIntellect.Infrastructure.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.PatientId);

        // 3. علاقة الـ One-to-One بين Patient و User
        builder.HasOne(p => p.User)
               .WithOne(u => u.Patient)
               .HasForeignKey<Patient>(p => p.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.MedicalHistory)
           .WithOne(m => m.Patient)
           .HasForeignKey<MedicalHistory>(m => m.PatientId) // هنا السحر: الـ FK بقى في جدول التاريخ المرضي
           .OnDelete(DeleteBehavior.Cascade); 

        builder.Property(p => p.Gender)
               .HasConversion<string>();
    }
}