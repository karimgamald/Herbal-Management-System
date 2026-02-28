using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Infrastructure.Configurations.EntitiesConfigurations;

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
               .WithOne() // 1-to-1
               .HasForeignKey<Patient>(p => p.MedicalHistoryId) // الـ FK في جدول الـ Patient
               .IsRequired(false) // عشان يسمح بـ Null (تخطي)
               .OnDelete(DeleteBehavior.SetNull); // لو مسحنا التاريخ المرضي، المريض ميتمسحش
    }
}