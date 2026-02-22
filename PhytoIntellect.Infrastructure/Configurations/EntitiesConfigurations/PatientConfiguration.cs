using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Infrastructure.Configurations.EntitiesConfigurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.PatientId);

        builder.HasOne(p => p.MedicalHistory)
               .WithOne()
               .HasForeignKey<Patient>(p => p.MedicalHistoryId)
               .IsRequired(); 

        // 3. علاقة الـ One-to-One بين Patient و User
        builder.HasOne(p => p.User)
               .WithOne(u => u.Patient)
               .HasForeignKey<Patient>(p => p.UserId)
               .OnDelete(DeleteBehavior.Cascade); 
    }
}