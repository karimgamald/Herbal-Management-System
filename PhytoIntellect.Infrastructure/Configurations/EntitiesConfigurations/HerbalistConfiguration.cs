using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Infrastructure.Configurations.EntitiesConfigurations
{
    public class HerbalistConfiguration : IEntityTypeConfiguration<Herbalist>
    {
        public void Configure(EntityTypeBuilder<Herbalist> builder)
        {
            builder.HasKey(h => h.HerbalistId);

            builder.HasOne(h => h.User)
                   .WithOne(u => u.Herbalist)
                   .HasForeignKey<Herbalist>(h => h.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            //is made in ManageHerbalistValidator
            //builder.HasIndex(h => h.LicenseNumber)
            //    .IsUnique();
        }
    }
}